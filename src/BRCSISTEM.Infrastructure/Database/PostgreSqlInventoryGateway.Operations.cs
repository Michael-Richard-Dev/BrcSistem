using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway
    {
        public void RegisterCount(DatabaseProfile profile, ConnectionResilienceSettings settings, RegisterInventoryCountRequest request)
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

                    var header = LoadLatestInventoryHeader(connection, transaction, request.InventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Leitura permitida apenas em inventario INICIADO ou EM_CONTAGEM.");
                    }

                    EnsurePointIsOpen(connection, transaction, request.InventoryNumber, request.PointId);
                    EnsureItemPlanned(connection, transaction, request.InventoryNumber, request.WarehouseCode, request.MaterialCode, request.LotCode);

                    if (!string.IsNullOrWhiteSpace(request.OriginUid))
                    {
                        using (var existsCommand = connection.CreateCommand())
                        {
                            existsCommand.Transaction = transaction;
                            existsCommand.CommandText = "SELECT COUNT(*) FROM inventario_contagens WHERE origem_uid = @origem_uid";
                            existsCommand.Parameters.Add(CreateParameter(existsCommand, "@origem_uid", request.OriginUid));
                            if (Convert.ToInt32(existsCommand.ExecuteScalar() ?? 0) > 0)
                            {
                                transaction.Commit();
                                return;
                            }
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO inventario_contagens
                                (numero, ponto_id, almoxarifado, material, lote, quantidade, usuario, ip_ponto, computador, dt_hr, status, origem_uid)
                            VALUES
                                (@numero, @ponto_id, @almoxarifado, @material, @lote, @quantidade, @usuario, @ip_ponto, @computador, @dt_hr, 'ATIVO', @origem_uid)";
                        command.Parameters.Add(CreateParameter(command, "@numero", request.InventoryNumber));
                        command.Parameters.Add(CreateParameter(command, "@ponto_id", request.PointId));
                        command.Parameters.Add(CreateParameter(command, "@almoxarifado", request.WarehouseCode));
                        command.Parameters.Add(CreateParameter(command, "@material", request.MaterialCode));
                        command.Parameters.Add(CreateParameter(command, "@lote", request.LotCode));
                        command.Parameters.Add(CreateParameter(command, "@quantidade", request.Quantity));
                        command.Parameters.Add(CreateParameter(command, "@usuario", request.ActorUserName));
                        command.Parameters.Add(CreateParameter(command, "@ip_ponto", string.IsNullOrWhiteSpace(request.IpAddress) ? (object)DBNull.Value : request.IpAddress));
                        command.Parameters.Add(CreateParameter(command, "@computador", string.IsNullOrWhiteSpace(request.ComputerName) ? (object)DBNull.Value : request.ComputerName));
                        command.Parameters.Add(CreateParameter(command, "@dt_hr", request.CountedAt));
                        command.Parameters.Add(CreateParameter(command, "@origem_uid", string.IsNullOrWhiteSpace(request.OriginUid) ? (object)DBNull.Value : request.OriginUid));
                        command.ExecuteNonQuery();
                    }

                    using (var heartbeatCommand = connection.CreateCommand())
                    {
                        heartbeatCommand.Transaction = transaction;
                        heartbeatCommand.CommandText = @"
                            UPDATE inventario_pontos
                               SET dt_heartbeat = @agora
                             WHERE numero = @numero
                               AND id = @id";
                        heartbeatCommand.Parameters.Add(CreateParameter(heartbeatCommand, "@agora", NowText()));
                        heartbeatCommand.Parameters.Add(CreateParameter(heartbeatCommand, "@numero", request.InventoryNumber));
                        heartbeatCommand.Parameters.Add(CreateParameter(heartbeatCommand, "@id", request.PointId));
                        heartbeatCommand.ExecuteNonQuery();
                    }

                    RecalculateCounts(connection, transaction, request.InventoryNumber);
                    if (string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateInventoryStatus(connection, transaction, request.InventoryNumber, "EM_CONTAGEM", openedAt: null, closedAt: null, finalizedAt: null, closedBy: null, canceledBy: null, cancellationReason: null);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void TouchPointHeartbeat(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE inventario_pontos
                       SET dt_heartbeat = @agora
                     WHERE numero = @numero
                       AND id = @id
                       AND status = 'ABERTO'";
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                command.Parameters.Add(CreateParameter(command, "@id", pointId));
                command.ExecuteNonQuery();
            }
        }

        public int ApplyZeroCounts(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName, string ipAddress, string computerName)
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

                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Acao permitida apenas em inventario INICIADO ou EM_CONTAGEM.");
                    }

                    EnsurePointIsOpen(connection, transaction, inventoryNumber, pointId);

                    var pendingItems = new List<Tuple<string, string, string>>();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            SELECT ii.almoxarifado, ii.material, ii.lote
                            FROM inventarios_itens ii
                            WHERE ii.numero = @numero
                              AND ii.status = 'ATIVO'
                              AND ii.quantidade_contada IS NULL";
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pendingItems.Add(Tuple.Create(
                                    ReadString(reader, "almoxarifado"),
                                    ReadString(reader, "material"),
                                    ReadString(reader, "lote")));
                            }
                        }
                    }

                    var inserted = 0;
                    foreach (var item in pendingItems)
                    {
                        using (var insertCommand = connection.CreateCommand())
                        {
                            insertCommand.Transaction = transaction;
                            insertCommand.CommandText = @"
                                INSERT INTO inventario_contagens
                                    (numero, ponto_id, almoxarifado, material, lote, quantidade, usuario, ip_ponto, computador, dt_hr, status, origem_uid)
                                VALUES
                                    (@numero, @ponto_id, @almoxarifado, @material, @lote, 0, @usuario, @ip_ponto, @computador, @dt_hr, 'ATIVO', @origem_uid)";
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@numero", inventoryNumber));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@ponto_id", pointId));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@almoxarifado", item.Item1));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@material", item.Item2));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@lote", item.Item3));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@usuario", actorUserName));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@ip_ponto", string.IsNullOrWhiteSpace(ipAddress) ? (object)DBNull.Value : ipAddress));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@computador", string.IsNullOrWhiteSpace(computerName) ? (object)DBNull.Value : computerName));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@dt_hr", NowText()));
                            insertCommand.Parameters.Add(CreateParameter(insertCommand, "@origem_uid", Guid.NewGuid().ToString("N")));
                            insertCommand.ExecuteNonQuery();
                            inserted++;
                        }
                    }

                    RecalculateCounts(connection, transaction, inventoryNumber);
                    if (inserted > 0 && string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateInventoryStatus(connection, transaction, inventoryNumber, "EM_CONTAGEM", openedAt: null, closedAt: null, finalizedAt: null, closedBy: null, canceledBy: null, cancellationReason: null);
                    }

                    transaction.Commit();
                    return inserted;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void StartInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName, bool allowEarlyStart)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventarios_itens");

                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "PENDENTE", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventario pendente salvo pode iniciar.");
                    }

                    if (!string.IsNullOrWhiteSpace(header.ScheduledAt)
                        && !allowEarlyStart
                        && DateTime.TryParse(header.ScheduledAt, out var scheduledAt)
                        && DateTime.Now < scheduledAt)
                    {
                        throw new InvalidOperationException("Inventario programado para abrir depois do horario atual.");
                    }

                    using (var countCommand = connection.CreateCommand())
                    {
                        countCommand.Transaction = transaction;
                        countCommand.CommandText = "SELECT COUNT(*) FROM inventarios_itens WHERE numero = @numero AND status = 'ATIVO'";
                        countCommand.Parameters.Add(CreateParameter(countCommand, "@numero", inventoryNumber));
                        if (Convert.ToInt32(countCommand.ExecuteScalar() ?? 0) <= 0)
                        {
                            throw new InvalidOperationException("Adicione pelo menos 1 item no planejamento antes de iniciar.");
                        }
                    }

                    UpdateInventoryStatus(connection, transaction, inventoryNumber, "INICIADO", openedAt: NowText(), closedAt: null, finalizedAt: null, closedBy: null, canceledBy: null, cancellationReason: null);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CloseInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName)
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

                    RecalculateCounts(connection, transaction, inventoryNumber);
                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventario EM_CONTAGEM pode ser fechado.");
                    }

                    EnsureNoOpenPoints(connection, transaction, inventoryNumber, "Nao e permitido fechar inventario com ponto aberto.");
                    EnsureAllItemsCounted(connection, transaction, inventoryNumber, "Existem itens sem contagem registrada.");

                    UpdateInventoryStatus(connection, transaction, inventoryNumber, "FECHADO", openedAt: null, closedAt: NowText(), finalizedAt: null, closedBy: actorUserName, canceledBy: null, cancellationReason: null);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ReopenInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "FECHADO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventario FECHADO pode ser reaberto.");
                    }

                    UpdateInventoryStatus(connection, transaction, inventoryNumber, "INICIADO", openedAt: header.OpenedAt, closedAt: string.Empty, finalizedAt: string.Empty, closedBy: string.Empty, canceledBy: string.Empty, cancellationReason: string.Empty);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static void EnsurePointIsOpen(DbConnection connection, DbTransaction transaction, string inventoryNumber, int pointId)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM inventario_pontos WHERE numero = @numero AND id = @id AND status = 'ABERTO'";
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                command.Parameters.Add(CreateParameter(command, "@id", pointId));
                if (Convert.ToInt32(command.ExecuteScalar() ?? 0) <= 0)
                {
                    throw new InvalidOperationException("Selecione um ponto ABERTO.");
                }
            }
        }

        private static void EnsureItemPlanned(DbConnection connection, DbTransaction transaction, string inventoryNumber, string warehouseCode, string materialCode, string lotCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(*)
                    FROM inventarios_itens
                    WHERE numero = @numero
                      AND almoxarifado = @almoxarifado
                      AND material = @material
                      AND lote = @lote
                      AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                if (Convert.ToInt32(command.ExecuteScalar() ?? 0) <= 0)
                {
                    throw new InvalidOperationException("Item nao pertence ao planejamento deste inventario.");
                }
            }
        }

        private static void EnsureNoOpenPoints(DbConnection connection, DbTransaction transaction, string inventoryNumber, string errorMessage)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM inventario_pontos WHERE numero = @numero AND status = 'ABERTO'";
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                if (Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0)
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private static void EnsureAllItemsCounted(DbConnection connection, DbTransaction transaction, string inventoryNumber, string errorMessage)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM inventarios_itens WHERE numero = @numero AND status = 'ATIVO' AND quantidade_contada IS NULL";
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                if (Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0)
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private static void UpdateInventoryStatus(
            DbConnection connection,
            DbTransaction transaction,
            string inventoryNumber,
            string status,
            string openedAt,
            string closedAt,
            string finalizedAt,
            string closedBy,
            string canceledBy,
            string cancellationReason)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE inventarios
                       SET status = @status,
                           dt_abertura = CASE WHEN @dt_abertura IS NULL THEN dt_abertura ELSE NULLIF(@dt_abertura, '') END,
                           dt_fechamento = CASE WHEN @dt_fechamento IS NULL THEN dt_fechamento ELSE NULLIF(@dt_fechamento, '') END,
                           dt_finalizacao = CASE WHEN @dt_finalizacao IS NULL THEN dt_finalizacao ELSE NULLIF(@dt_finalizacao, '') END,
                           fechado_por = CASE WHEN @fechado_por IS NULL THEN fechado_por ELSE NULLIF(@fechado_por, '') END,
                           cancelado_por = CASE WHEN @cancelado_por IS NULL THEN cancelado_por ELSE NULLIF(@cancelado_por, '') END,
                           motivo_cancelamento = CASE WHEN @motivo IS NULL THEN motivo_cancelamento ELSE NULLIF(@motivo, '') END,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND versao = (
                           SELECT MAX(versao)
                           FROM inventarios x
                           WHERE x.numero = @numero
                       )";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@dt_abertura", openedAt == null ? (object)DBNull.Value : openedAt));
                command.Parameters.Add(CreateParameter(command, "@dt_fechamento", closedAt == null ? (object)DBNull.Value : closedAt));
                command.Parameters.Add(CreateParameter(command, "@dt_finalizacao", finalizedAt == null ? (object)DBNull.Value : finalizedAt));
                command.Parameters.Add(CreateParameter(command, "@fechado_por", closedBy == null ? (object)DBNull.Value : closedBy));
                command.Parameters.Add(CreateParameter(command, "@cancelado_por", canceledBy == null ? (object)DBNull.Value : canceledBy));
                command.Parameters.Add(CreateParameter(command, "@motivo", cancellationReason == null ? (object)DBNull.Value : cancellationReason));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                command.ExecuteNonQuery();
            }
        }
    }
}
