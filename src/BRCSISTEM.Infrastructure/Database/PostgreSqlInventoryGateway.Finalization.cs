using System;
using System.Collections.Generic;
using System.Data;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway
    {
        public int FinalizeInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventarios_itens");
                    LockTable(connection, transaction, "inventario_pontos");
                    LockTable(connection, transaction, "inventario_contagens");
                    LockTable(connection, transaction, "movimentos_estoque");
                    LockTable(connection, transaction, "registro_bloqueios");

                    RecalculateCounts(connection, transaction, inventoryNumber);
                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "FECHADO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventario FECHADO pode ser encerrado.");
                    }

                    EnsureNoOpenPoints(connection, transaction, inventoryNumber, "Nao e permitido encerrar inventario com ponto aberto.");
                    EnsureAllItemsCounted(connection, transaction, inventoryNumber, "Existem itens sem contagem registrada.");

                    using (var deactivateCommand = connection.CreateCommand())
                    {
                        deactivateCommand.Transaction = transaction;
                        deactivateCommand.CommandText = @"
                            UPDATE movimentos_estoque
                               SET status = 'INATIVO',
                                   dt_hr_alteracao = @agora
                             WHERE documento_tipo = 'INVENTARIO'
                               AND documento_numero = @numero
                               AND status = 'ATIVO'";
                        deactivateCommand.Parameters.Add(CreateParameter(deactivateCommand, "@agora", NowText()));
                        deactivateCommand.Parameters.Add(CreateParameter(deactivateCommand, "@numero", inventoryNumber));
                        deactivateCommand.ExecuteNonQuery();
                    }

                    var finalizedAt = NowText();
                    var adjustmentCount = 0;
                    var items = new List<FinalizeItemRecord>();
                    using (var itemCommand = connection.CreateCommand())
                    {
                        itemCommand.Transaction = transaction;
                        itemCommand.CommandText = @"
                            SELECT almoxarifado,
                                   material,
                                   lote,
                                   COALESCE(quantidade_contada, 0) AS quantidade_contada
                            FROM inventarios_itens
                            WHERE numero = @numero
                              AND status = 'ATIVO'
                            ORDER BY almoxarifado, material, lote";
                        itemCommand.Parameters.Add(CreateParameter(itemCommand, "@numero", inventoryNumber));

                        using (var reader = itemCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new FinalizeItemRecord
                                {
                                    WarehouseCode = ReadString(reader, "almoxarifado"),
                                    MaterialCode = ReadString(reader, "material"),
                                    LotCode = ReadString(reader, "lote"),
                                    CountedQuantity = ReadDecimal(reader, "quantidade_contada"),
                                });
                            }
                        }
                    }

                    foreach (var item in items)
                    {
                        var systemBalance = GetStockBalanceAtInternal(connection, transaction, item.WarehouseCode, item.MaterialCode, item.LotCode, finalizedAt);
                        var delta = item.CountedQuantity - systemBalance;
                        var adjustmentType = delta > 0M ? "ENTRADA" : (delta < 0M ? "REQUISICAO" : "SEM_AJUSTE");

                        using (var updateItem = connection.CreateCommand())
                        {
                            updateItem.Transaction = transaction;
                            updateItem.CommandText = @"
                                UPDATE inventarios_itens
                                   SET saldo_sistema = @saldo_sistema,
                                       ajuste = @ajuste,
                                       tipo_ajuste = @tipo_ajuste,
                                       dt_hr_alteracao = @agora
                                 WHERE numero = @numero
                                   AND almoxarifado = @almoxarifado
                                   AND material = @material
                                   AND lote = @lote
                                   AND status = 'ATIVO'";
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@saldo_sistema", systemBalance));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@ajuste", delta));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@tipo_ajuste", adjustmentType));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@agora", NowText()));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@numero", inventoryNumber));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@almoxarifado", item.WarehouseCode));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@material", item.MaterialCode));
                            updateItem.Parameters.Add(CreateParameter(updateItem, "@lote", item.LotCode));
                            updateItem.ExecuteNonQuery();
                        }

                        if (Math.Abs(delta) <= 0.0000001M)
                        {
                            continue;
                        }

                        adjustmentCount++;
                        using (var movementCommand = connection.CreateCommand())
                        {
                            movementCommand.Transaction = transaction;
                            movementCommand.CommandText = @"
                                INSERT INTO movimentos_estoque
                                    (documento_numero, documento_tipo, documento_item, data_movimento, tipo, fornecedor, almoxarifado, material, lote, quantidade, produto_utilizado, vencimento, usuario, status, dt_hr_criacao, dt_hr_alteracao)
                                VALUES
                                    (@numero, 'INVENTARIO', @item, @data_movimento, @tipo, @fornecedor, @almoxarifado, @material, @lote, @quantidade, NULL, @vencimento, @usuario, 'ATIVO', @agora, @agora)";
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@numero", inventoryNumber));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@item", adjustmentCount));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@data_movimento", finalizedAt));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@tipo", adjustmentType));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@fornecedor", GetLotSupplier(connection, transaction, item.LotCode, item.MaterialCode)));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@almoxarifado", item.WarehouseCode));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@material", item.MaterialCode));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@lote", item.LotCode));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@quantidade", Math.Abs(delta)));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@vencimento", GetLotExpiration(connection, transaction, item.LotCode, item.MaterialCode)));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@usuario", actorUserName));
                            movementCommand.Parameters.Add(CreateParameter(movementCommand, "@agora", NowText()));
                            movementCommand.ExecuteNonQuery();
                        }
                    }

                    UpdateInventoryStatus(connection, transaction, inventoryNumber, "ENCERRADO", openedAt: null, closedAt: null, finalizedAt: finalizedAt, closedBy: null, canceledBy: string.Empty, cancellationReason: string.Empty);
                    ReleaseLockInternal(connection, transaction, inventoryNumber, actorUserName, updateHeader: true);
                    transaction.Commit();
                    return adjustmentCount;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CancelInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName, string reason)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventario_pontos");
                    LockTable(connection, transaction, "registro_bloqueios");

                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "PENDENTE", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "FECHADO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventario pendente, iniciado, em contagem ou fechado pode ser cancelado.");
                    }

                    EnsureNoOpenPoints(connection, transaction, inventoryNumber, "Feche os pontos abertos antes de cancelar.");
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE inventarios
                               SET status = 'CANCELADO',
                                   dt_fechamento = @agora,
                                   dt_finalizacao = @agora,
                                   cancelado_por = @usuario,
                                   motivo_cancelamento = @motivo,
                                   dt_hr_alteracao = @agora
                             WHERE numero = @numero
                               AND versao = (
                                   SELECT MAX(versao)
                                   FROM inventarios x
                                   WHERE x.numero = @numero
                               )";
                        command.Parameters.Add(CreateParameter(command, "@agora", now));
                        command.Parameters.Add(CreateParameter(command, "@usuario", actorUserName));
                        command.Parameters.Add(CreateParameter(command, "@motivo", string.IsNullOrWhiteSpace(reason) ? (object)DBNull.Value : reason));
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                        command.ExecuteNonQuery();
                    }

                    ReleaseLockInternal(connection, transaction, inventoryNumber, actorUserName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private sealed class FinalizeItemRecord
        {
            public string WarehouseCode { get; set; }

            public string MaterialCode { get; set; }

            public string LotCode { get; set; }

            public decimal CountedQuantity { get; set; }
        }
    }
}
