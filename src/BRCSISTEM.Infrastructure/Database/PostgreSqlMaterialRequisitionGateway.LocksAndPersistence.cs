using System;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlMaterialRequisitionGateway
    {
        public RecordLockResult TryLockRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var header = LoadLatestRequisitionHeader(connection, transaction, number);
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
                            Message = "A requisicao " + number + " esta em edicao por " + header.LockedBy + ".",
                        };
                    }

                    ReleaseLockInternal(connection, transaction, number, null, updateHeader: false);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE requisicoes
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
                                ('requisicoes', @chave, @usuario, CURRENT_TIMESTAMP, TRUE, @observacoes, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                        command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number)));
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@observacoes", "Bloqueio da requisicao " + number));
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

        public void ReleaseRequisitionLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes IN EXCLUSIVE MODE");
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

        public void CreateRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveMaterialRequisitionRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");

                    var current = LoadLatestRequisitionHeader(connection, transaction, request.Number);
                    if (current != null && string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A requisicao " + request.Number + " ja esta ativa.");
                    }

                    var nextVersion = current == null ? 1 : current.Version + 1;
                    InsertRequisitionHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertRequisitionItems(connection, transaction, request, nextVersion);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveMaterialRequisitionRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestRequisitionHeader(connection, transaction, request.Number);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Requisicao nao encontrada para alteracao.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, request.ActorUserName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A requisicao " + request.Number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateRequisitionHeaderStatus(connection, transaction, request.Number, current.Version, "INATIVO");
                    UpdateRequisitionItemsStatus(connection, transaction, request.Number, current.Version, "INATIVO");
                    UpdateRequisitionMovementsStatus(connection, transaction, request.Number, "INATIVO");

                    var nextVersion = current.Version + 1;
                    InsertRequisitionHeader(connection, transaction, request, nextVersion, "ATIVO");
                    InsertRequisitionItems(connection, transaction, request, nextVersion);
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

        public void CancelRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE requisicoes_itens IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE movimentos_estoque IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE registro_bloqueios IN EXCLUSIVE MODE");

                    var current = LoadLatestRequisitionHeader(connection, transaction, number);
                    if (current == null || !string.Equals(current.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Requisicao nao encontrada ou ja cancelada.");
                    }

                    if (!string.IsNullOrWhiteSpace(current.LockedBy)
                        && !string.Equals(current.LockedBy, userName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("A requisicao " + number + " esta em edicao por " + current.LockedBy + ".");
                    }

                    UpdateRequisitionHeaderStatus(connection, transaction, number, current.Version, "CANCELADO");
                    UpdateRequisitionItemsStatus(connection, transaction, number, current.Version, "CANCELADO");
                    UpdateRequisitionMovementsStatus(connection, transaction, number, "INATIVO");
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

        private static void InsertRequisitionHeader(DbConnection connection, DbTransaction transaction, SaveMaterialRequisitionRequest request, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO requisicoes
                        (numero, dt_movimento, almoxarifado, status, versao, bloqueado_por, bloqueado_em, dt_hr_criacao, dt_hr_alteracao)
                    VALUES
                        (@numero, @dt_movimento, @almoxarifado, @status, @versao, NULL, NULL, @agora, @agora)";
                command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                command.Parameters.Add(CreateParameter(command, "@dt_movimento", request.MovementDateTime));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", request.WarehouseCode));
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@versao", version));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.ExecuteNonQuery();
            }
        }

        private static void InsertRequisitionItems(DbConnection connection, DbTransaction transaction, SaveMaterialRequisitionRequest request, int version)
        {
            var itemNumber = 0;
            foreach (var item in request.Items)
            {
                itemNumber++;
                using (var itemCommand = connection.CreateCommand())
                {
                    itemCommand.Transaction = transaction;
                    itemCommand.CommandText = @"
                        INSERT INTO requisicoes_itens
                            (numero, item_numero, material, lote, almoxarifado, quantidade, status, versao, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, @item_numero, @material, @lote, @almoxarifado, @quantidade, 'ATIVO', @versao, @agora, @agora)";
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@numero", request.Number));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@item_numero", itemNumber));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@material", item.MaterialCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@lote", item.LotCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@almoxarifado", request.WarehouseCode));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@quantidade", item.Quantity));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@versao", version));
                    itemCommand.Parameters.Add(CreateParameter(itemCommand, "@agora", NowText()));
                    itemCommand.ExecuteNonQuery();
                }

                InsertRequisitionMovement(connection, transaction, request, item, itemNumber);
            }
        }

        private static void InsertRequisitionMovement(
            DbConnection connection,
            DbTransaction transaction,
            SaveMaterialRequisitionRequest request,
            MaterialRequisitionItemInput item,
            int itemNumber)
        {
            using (var movementCommand = connection.CreateCommand())
            {
                movementCommand.Transaction = transaction;
                movementCommand.CommandText = @"
                    INSERT INTO movimentos_estoque
                        (documento_numero, documento_tipo, documento_item, data_movimento, tipo, fornecedor, almoxarifado, material, lote, quantidade, produto_utilizado, vencimento, usuario, status, dt_hr_criacao, dt_hr_alteracao)
                    VALUES
                        (@numero, 'REQUISICAO', @item, @data_movimento, 'REQUISICAO', @fornecedor, @almoxarifado, @material, @lote, @quantidade, NULL, @vencimento, @usuario, 'ATIVO', @agora, @agora)";
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@numero", request.Number));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@item", itemNumber));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@data_movimento", request.MovementDateTime));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@fornecedor", GetLotSupplier(connection, transaction, item.LotCode)));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@almoxarifado", request.WarehouseCode));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@material", item.MaterialCode));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@lote", item.LotCode));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@quantidade", item.Quantity));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@vencimento", GetLotExpiration(connection, transaction, item.LotCode)));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@usuario", request.ActorUserName));
                movementCommand.Parameters.Add(CreateParameter(movementCommand, "@agora", NowText()));
                movementCommand.ExecuteNonQuery();
            }
        }

        private static void UpdateRequisitionHeaderStatus(DbConnection connection, DbTransaction transaction, string number, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE requisicoes
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

        private static void UpdateRequisitionItemsStatus(DbConnection connection, DbTransaction transaction, string number, int version, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE requisicoes_itens
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

        private static void UpdateRequisitionMovementsStatus(DbConnection connection, DbTransaction transaction, string number, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE movimentos_estoque
                       SET status = @status,
                           dt_hr_alteracao = @agora
                     WHERE documento_numero = @numero
                       AND documento_tipo = 'REQUISICAO'
                       AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@status", status));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.ExecuteNonQuery();
            }
        }
    }
}
