using System;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInboundReceiptGateway
    {
        public RecordLockResult TryLockReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var header = LoadLatestNoteHeader(connection, transaction, number, supplierCode);
                    if (header == null || !string.Equals(header.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction.Commit();
                        return new RecordLockResult { Success = true };
                    }

                    if (!string.IsNullOrWhiteSpace(header.LockedBy)
                        && !string.Equals(header.LockedBy, userName, StringComparison.OrdinalIgnoreCase))
                    {
                        transaction.Commit();
                        return new RecordLockResult
                        {
                            Success = false,
                            Message = "A nota " + number + " esta em edicao por " + header.LockedBy + ".",
                        };
                    }

                    ReleaseLockInternal(connection, transaction, number, supplierCode, null, updateHeader: false);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE notas
                               SET bloqueado_por = @usuario,
                                   bloqueado_em = @agora,
                                   dt_hr_alteracao = @agora
                             WHERE numero = @numero
                               AND fornecedor = @fornecedor
                               AND versao = @versao";
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                        command.Parameters.Add(CreateParameter(command, "@numero", number));
                        command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                        command.Parameters.Add(CreateParameter(command, "@versao", header.Version));
                        command.ExecuteNonQuery();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO registro_bloqueios
                                (tabela, registro_chave, usuario, data_bloqueio, ativo, observacoes, dt_hr_criacao, dt_hr_alteracao)
                            VALUES
                                ('notas', @chave, @usuario, CURRENT_TIMESTAMP, TRUE, @observacoes, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                        command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number, supplierCode)));
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@observacoes", "Bloqueio da nota de entrada " + number));
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return new RecordLockResult { Success = true };
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    return new RecordLockResult { Success = false, Message = exception.Message };
                }
            }
        }

        public void ReleaseReceiptLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");
                    ReleaseLockInternal(connection, transaction, number, supplierCode, userName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CreateReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInboundReceiptRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");

                    var current = LoadLatestNoteHeader(connection, transaction, request.Number, request.SupplierCode);
                    if (current != null && string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A nota " + request.Number + " ja esta ativa para este fornecedor.");
                    }

                    var nextVersion = GetNextReceiptVersion(connection, transaction, request.Number, request.SupplierCode);
                    InsertReceiptHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertReceiptItems(connection, transaction, request, nextVersion);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInboundReceiptRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestNoteHeader(connection, transaction, request.Number, request.SupplierCode);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Nota nao encontrada para alteracao.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, request.ActorUserName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A nota " + request.Number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateReceiptStatus(connection, transaction, request.Number, request.SupplierCode, current.Version, "INATIVO");
                    UpdateReceiptItemsStatus(connection, transaction, request.Number, request.SupplierCode, current.Version, "INATIVO");
                    UpdateReceiptMovementsStatus(connection, transaction, request.Number, request.SupplierCode, "INATIVO");

                    var nextVersion = current.Version + 1;
                    InsertReceiptHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertReceiptItems(connection, transaction, request, nextVersion);
                    ReleaseLockInternal(connection, transaction, request.Number, request.SupplierCode, request.ActorUserName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CancelReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE notas_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestNoteHeader(connection, transaction, number, supplierCode);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Nota nao encontrada ou ja cancelada.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, userName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A nota " + number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateReceiptStatus(connection, transaction, number, supplierCode, current.Version, "CANCELADA");
                    UpdateReceiptItemsStatus(connection, transaction, number, supplierCode, current.Version, "CANCELADA");
                    UpdateReceiptMovementsStatus(connection, transaction, number, supplierCode, "CANCELADA");
                    ReleaseLockInternal(connection, transaction, number, supplierCode, userName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static void InsertReceiptHeader(DbConnection connection, DbTransaction transaction, SaveInboundReceiptRequest request, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO notas
                        (numero, fornecedor, almoxarifado, dt_emissao, dt_movimento, usuario, status, versao, bloqueado_por, bloqueado_em, dt_hr_criacao, dt_hr_alteracao)
                    VALUES
                        (@numero, @fornecedor, @almoxarifado, @dt_emissao, @dt_movimento, @usuario, @status, @versao, NULL, NULL, @agora, @agora)";
                command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", request.SupplierCode));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", request.WarehouseCode));
                command.Parameters.Add(CreateParameter(command, "@dt_emissao", request.EmissionDate));
                command.Parameters.Add(CreateParameter(command, "@dt_movimento", request.ReceiptDateTime));
                command.Parameters.Add(CreateParameter(command, "@usuario", request.ActorUserName));
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.ExecuteNonQuery();
            }
        }

        private static void InsertReceiptItems(DbConnection connection, DbTransaction transaction, SaveInboundReceiptRequest request, int version)
        {
            var itemNumber = 0;
            foreach (var item in request.Items)
            {
                itemNumber++;
                using (var itemCommand = connection.CreateCommand())
                {
                    itemCommand.Transaction = transaction;
                    itemCommand.CommandText = @"
                        INSERT INTO notas_itens
                            (numero, material, fornecedor, lote, almoxarifado, quantidade, status, versao, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, @material, @fornecedor, @lote, @almoxarifado, @quantidade, 'ATIVO', @versao, @agora, @agora)";
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@numero", request.Number));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@material", item.MaterialCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@fornecedor", request.SupplierCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@lote", item.LotCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@almoxarifado", request.WarehouseCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@quantidade", item.Quantity));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@versao", version));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@agora", NowText()));
                    itemCommand.ExecuteNonQuery();
                }

                using (var movementCommand = connection.CreateCommand())
                {
                    movementCommand.Transaction = transaction;
                    movementCommand.CommandText = @"
                        INSERT INTO movimentos_estoque
                            (documento_numero, documento_tipo, documento_item, data_movimento, tipo, fornecedor, almoxarifado, material, lote, quantidade, vencimento, usuario, status, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, 'NOTA', @item, @data_movimento, 'ENTRADA', @fornecedor, @almoxarifado, @material, @lote, @quantidade, @vencimento, @usuario, 'ATIVO', @agora, @agora)";
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@numero", request.Number));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@item", itemNumber));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@data_movimento", request.ReceiptDateTime));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@fornecedor", request.SupplierCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@almoxarifado", request.WarehouseCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@material", item.MaterialCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@lote", item.LotCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@quantidade", item.Quantity));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@vencimento", GetLotExpiration(connection, transaction, item.LotCode, request.SupplierCode)));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@usuario", request.ActorUserName));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@agora", NowText()));
                    movementCommand.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateReceiptStatus(DbConnection connection, DbTransaction transaction, string number, string supplierCode, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE notas
                       SET status = @status,
                           bloqueado_por = NULL,
                           bloqueado_em = NULL,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND fornecedor = @fornecedor
                       AND versao = @versao";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateReceiptItemsStatus(DbConnection connection, DbTransaction transaction, string number, string supplierCode, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE notas_itens
                       SET status = @status,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND fornecedor = @fornecedor
                       AND versao = @versao";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateReceiptMovementsStatus(DbConnection connection, DbTransaction transaction, string number, string supplierCode, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE movimentos_estoque
                       SET status = @status,
                           dt_hr_alteracao = @agora
                     WHERE documento_numero = @numero
                       AND documento_tipo = 'NOTA'
                       AND fornecedor = @fornecedor
                       AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                command.ExecuteNonQuery();
            }
        }
    }
}
