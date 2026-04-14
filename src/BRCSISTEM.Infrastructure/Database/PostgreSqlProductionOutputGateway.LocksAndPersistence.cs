using System;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlProductionOutputGateway
    {
        public RecordLockResult TryLockOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var header = LoadLatestOutputHeader(connection, transaction, number);
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
                            Message = "A saida " + number + " esta em edicao por " + header.LockedBy + ".",
                        };
                    }

                    ReleaseLockInternal(connection, transaction, number, null, updateHeader: false);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE saidas_producao
                               SET bloqueado_por = @usuario,
                                   bloqueado_em = @agora,
                                   dt_hr_alteracao = @agora
                             WHERE numero = @numero
                               AND versao = @versao";
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                        command.Parameters.Add(CreateParameter(command, "@numero", number));
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
                                ('saidas_producao', @chave, @usuario, CURRENT_TIMESTAMP, TRUE, @observacoes, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                        command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number)));
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@observacoes", "Bloqueio da saida de producao " + number));
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

        public void ReleaseOutputLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");
                    ReleaseLockInternal(connection, transaction, number, userName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CreateOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductionOutputRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");

                    var current = LoadLatestOutputHeader(connection, transaction, request.Number);
                    if (current != null && string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A saida " + request.Number + " ja esta ativa.");
                    }

                    var nextVersion = current == null ? 1 : current.Version + 1;
                    InsertOutputHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertOutputItems(connection, transaction, request, nextVersion);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductionOutputRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestOutputHeader(connection, transaction, request.Number);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Saida de producao nao encontrada para alteracao.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, request.ActorUserName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A saida " + request.Number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateOutputHeaderStatus(connection, transaction, request.Number, current.Version, "INATIVO");
                    UpdateOutputItemsStatus(connection, transaction, request.Number, current.Version, "INATIVO");
                    UpdateOutputMovementsStatus(connection, transaction, request.Number, "INATIVO");

                    var nextVersion = current.Version + 1;
                    InsertOutputHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertOutputItems(connection, transaction, request, nextVersion);
                    ReleaseLockInternal(connection, transaction, request.Number, request.ActorUserName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CancelOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE saidas_producao_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestOutputHeader(connection, transaction, number);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Saida de producao nao encontrada ou ja cancelada.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, userName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A saida " + number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateOutputHeaderStatus(connection, transaction, number, current.Version, "CANCELADO");
                    UpdateOutputItemsStatus(connection, transaction, number, current.Version, "CANCELADO");
                    UpdateOutputMovementsStatus(connection, transaction, number, "INATIVO");
                    ReleaseLockInternal(connection, transaction, number, userName, updateHeader: true);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static void InsertOutputHeader(DbConnection connection, DbTransaction transaction, SaveProductionOutputRequest request, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO saidas_producao
                        (numero, finalidade, dt_movimento, turno, status, versao, bloqueado_por, bloqueado_em, dt_hr_criacao, dt_hr_alteracao)
                    VALUES
                        (@numero, @finalidade, @dt_movimento, @turno, @status, @versao, NULL, NULL, @agora, @agora)";
                command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                command.Parameters.Add(CreateParameter(command, "@finalidade", request.Purpose));
                command.Parameters.Add(CreateParameter(command, "@dt_movimento", request.MovementDateTime));
                command.Parameters.Add(CreateParameter(command, "@turno", request.Shift));
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.ExecuteNonQuery();
            }
        }

        private static void InsertOutputItems(DbConnection connection, DbTransaction transaction, SaveProductionOutputRequest request, int version)
        {
            var itemNumber = 0;
            foreach (var item in request.Items)
            {
                itemNumber++;
                using (var itemCommand = connection.CreateCommand())
                {
                    itemCommand.Transaction = transaction;
                    itemCommand.CommandText = @"
                        INSERT INTO saidas_producao_itens
                            (numero, produto, material, lote, almoxarifado, quantidade, qtd_envio, qtd_retorno, qtd_consumida, status, versao, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, @produto, @material, @lote, @almoxarifado, @quantidade, @qtd_envio, @qtd_retorno, @qtd_consumida, 'ATIVO', @versao, @agora, @agora)";
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@numero", request.Number));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@produto", string.IsNullOrWhiteSpace(item.ProductCode) ? (object)DBNull.Value : item.ProductCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@material", item.MaterialCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@lote", item.LotCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@almoxarifado", request.WarehouseCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@quantidade", item.QuantityConsumed));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@qtd_envio", item.QuantitySent));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@qtd_retorno", item.QuantityReturned));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@qtd_consumida", item.QuantityConsumed));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@versao", version));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@agora", NowText()));
                    itemCommand.ExecuteNonQuery();
                }

                using (var movementCommand = connection.CreateCommand())
                {
                    movementCommand.Transaction = transaction;
                    movementCommand.CommandText = @"
                        INSERT INTO movimentos_estoque
                            (documento_numero, documento_tipo, documento_item, data_movimento, tipo, fornecedor, almoxarifado, material, lote, quantidade, produto_utilizado, vencimento, usuario, status, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, 'SAIDA_PRODUCAO', @item, @data_movimento, 'SAIDA_PRODUCAO', @fornecedor, @almoxarifado, @material, @lote, @quantidade, @produto, @vencimento, @usuario, 'ATIVO', @agora, @agora)";
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@numero", request.Number));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@item", itemNumber));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@data_movimento", request.MovementDateTime));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@fornecedor", GetLotSupplier(connection, transaction, item.LotCode)));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@almoxarifado", request.WarehouseCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@material", item.MaterialCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@lote", item.LotCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@quantidade", item.QuantityConsumed));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@produto", string.IsNullOrWhiteSpace(item.ProductCode) ? (object)DBNull.Value : item.ProductCode));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@vencimento", GetLotExpiration(connection, transaction, item.LotCode)));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@usuario", request.ActorUserName));
                    movementCommand.Parameters.Add(CreateParameter(movementCommand, "@agora", NowText()));
                    movementCommand.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateOutputHeaderStatus(DbConnection connection, DbTransaction transaction, string number, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE saidas_producao
                       SET status = @status,
                           bloqueado_por = NULL,
                           bloqueado_em = NULL,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND versao = @versao";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateOutputItemsStatus(DbConnection connection, DbTransaction transaction, string number, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE saidas_producao_itens
                       SET status = @status,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND versao = @versao";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateOutputMovementsStatus(DbConnection connection, DbTransaction transaction, string number, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE movimentos_estoque
                       SET status = @status,
                           dt_hr_alteracao = @agora
                     WHERE documento_numero = @numero
                       AND documento_tipo = 'SAIDA_PRODUCAO'
                       AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.ExecuteNonQuery();
            }
        }
    }
}
