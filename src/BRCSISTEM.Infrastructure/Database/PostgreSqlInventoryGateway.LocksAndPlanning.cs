using System;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway
    {
        public RecordLockResult TryLockInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "registro_bloqueios");

                    var header = LoadLatestInventoryHeader(connection, transaction, number);
                    if (header == null
                        || string.Equals(header.Status, "ENCERRADO", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(header.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase))
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
                            Message = "O inventario " + number + " esta em edicao por " + header.LockedBy + ".",
                        };
                    }

                    ReleaseLockInternal(connection, transaction, number, null, updateHeader: false);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE inventarios
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
                                ('inventarios', @chave, @usuario, CURRENT_TIMESTAMP, TRUE, @observacoes, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                        command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number)));
                        command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                        command.Parameters.Add(CreateParameter(command, "@observacoes", "Bloqueio do inventario " + number));
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

        public void ReleaseInventoryLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "registro_bloqueios");
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

        public void CreateInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInventoryRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventarios_itens");
                    LockTable(connection, transaction, "inventario_pontos");

                    var current = LoadLatestInventoryHeader(connection, transaction, request.Number);
                    if (current != null && !string.Equals(current.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("O inventario " + request.Number + " ja existe.");
                    }

                    InsertInventoryHeader(connection, transaction, request);
                    ReplaceInventoryItems(connection, transaction, request);
                    ReplaceInventoryPoints(connection, transaction, request);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateInventoryPlanning(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInventoryRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventarios_itens");
                    LockTable(connection, transaction, "inventario_pontos");

                    var current = LoadLatestInventoryHeader(connection, transaction, request.Number);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado para alteracao.");
                    }

                    if (!string.Equals(current.Status, "PENDENTE", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Somente inventarios pendentes podem ter o planejamento alterado.");
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE inventarios
                               SET dt_movimento = @dt_movimento,
                                   observacao = @observacao,
                                   dt_inicio = @dt_inicio,
                                   max_pontos = @max_pontos,
                                   dt_hr_alteracao = @agora
                             WHERE numero = @numero
                               AND versao = @versao";
                        command.Parameters.Add(CreateParameter(command, "@dt_movimento", request.CreatedDateTime));
                        command.Parameters.Add(CreateParameter(command, "@observacao", string.IsNullOrWhiteSpace(request.Observation) ? (object)DBNull.Value : request.Observation));
                        command.Parameters.Add(CreateParameter(command, "@dt_inicio", string.IsNullOrWhiteSpace(request.ScheduledDateTime) ? (object)DBNull.Value : request.ScheduledDateTime));
                        command.Parameters.Add(CreateParameter(command, "@max_pontos", request.MaxOpenPoints));
                        command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                        command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                        command.Parameters.Add(CreateParameter(command, "@versao", current.Version));
                        command.ExecuteNonQuery();
                    }

                    ReplaceInventoryItems(connection, transaction, request);
                    ReplaceInventoryPoints(connection, transaction, request);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public InventoryPointSummary AddPoint(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string inventoryNumber,
            InventoryPointInput point,
            string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventario_pontos");

                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "PENDENTE", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Inventario deve estar PENDENTE ou EM_CONTAGEM.");
                    }

                    EnsurePointAvailability(connection, transaction, inventoryNumber, header.MaxOpenPoints, point.IpAddress, null);
                    var pointId = InsertPoint(connection, transaction, inventoryNumber, point, actorUserName, point.Status);
                    transaction.Commit();

                    return new InventoryPointSummary
                    {
                        Id = pointId,
                        PointName = point.PointName,
                        IpAddress = point.IpAddress,
                        ComputerName = point.ComputerName,
                        OpenedBy = actorUserName,
                        Status = string.IsNullOrWhiteSpace(point.Status) ? "ABERTO" : point.Status,
                        OpenedAt = NowText(),
                        HeartbeatAt = NowText(),
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ClosePoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventario_pontos");
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE inventario_pontos
                               SET status = 'FECHADO',
                                   usuario_fechamento = @usuario,
                                   dt_fechamento = @agora
                             WHERE numero = @numero
                               AND id = @id
                               AND status = 'ABERTO'";
                        command.Parameters.Add(CreateParameter(command, "@usuario", actorUserName));
                        command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                        command.Parameters.Add(CreateParameter(command, "@id", pointId));
                        if (command.ExecuteNonQuery() <= 0)
                        {
                            throw new InvalidOperationException("Selecione um ponto ABERTO.");
                        }
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

        public void ReopenPoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventarios");
                    LockTable(connection, transaction, "inventario_pontos");

                    var header = LoadLatestInventoryHeader(connection, transaction, inventoryNumber);
                    if (header == null)
                    {
                        throw new InvalidOperationException("Inventario nao encontrado.");
                    }

                    if (!string.Equals(header.Status, "INICIADO", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(header.Status, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Reabertura de ponto permitida apenas em INICIADO ou EM_CONTAGEM.");
                    }

                    string ipAddress;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "SELECT COALESCE(ip_ponto, '') FROM inventario_pontos WHERE numero = @numero AND id = @id";
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                        command.Parameters.Add(CreateParameter(command, "@id", pointId));
                        var result = command.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new InvalidOperationException("Ponto nao encontrado.");
                        }

                        ipAddress = Convert.ToString(result);
                    }

                    EnsurePointAvailability(connection, transaction, inventoryNumber, header.MaxOpenPoints, ipAddress, pointId);

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            UPDATE inventario_pontos
                               SET status = 'ABERTO',
                                   usuario_fechamento = NULL,
                                   dt_fechamento = NULL,
                                   dt_heartbeat = @agora
                             WHERE numero = @numero
                               AND id = @id
                               AND status = 'FECHADO'";
                        command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                        command.Parameters.Add(CreateParameter(command, "@id", pointId));
                        if (command.ExecuteNonQuery() <= 0)
                        {
                            throw new InvalidOperationException("Selecione um ponto FECHADO.");
                        }
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

        public void DeletePoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    LockTable(connection, transaction, "inventario_pontos");
                    LockTable(connection, transaction, "inventario_contagens");

                    using (var countCommand = connection.CreateCommand())
                    {
                        countCommand.Transaction = transaction;
                        countCommand.CommandText = "SELECT COUNT(*) FROM inventario_contagens WHERE numero = @numero AND ponto_id = @id AND status = 'ATIVO'";
                        countCommand.Parameters.Add(CreateParameter(countCommand, "@numero", inventoryNumber));
                        countCommand.Parameters.Add(CreateParameter(countCommand, "@id", pointId));
                        if (Convert.ToInt32(countCommand.ExecuteScalar() ?? 0) > 0)
                        {
                            throw new InvalidOperationException("Nao e permitido excluir ponto com leituras registradas.");
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "DELETE FROM inventario_pontos WHERE numero = @numero AND id = @id";
                        command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                        command.Parameters.Add(CreateParameter(command, "@id", pointId));
                        if (command.ExecuteNonQuery() <= 0)
                        {
                            throw new InvalidOperationException("Ponto nao encontrado.");
                        }
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

        private static void LockTable(DbConnection connection, DbTransaction transaction, string tableName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "LOCK TABLE " + tableName + " IN EXCLUSIVE MODE";
                command.ExecuteNonQuery();
            }
        }

        private static void InsertInventoryHeader(DbConnection connection, DbTransaction transaction, SaveInventoryRequest request)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO inventarios
                        (numero, dt_movimento, observacao, usuario, status, versao, dt_hr_criacao, dt_hr_alteracao, dt_inicio, max_pontos, bloqueado_por, bloqueado_em)
                    VALUES
                        (@numero, @dt_movimento, @observacao, @usuario, 'PENDENTE', 1, @dt_hr_criacao, @dt_hr_alteracao, @dt_inicio, @max_pontos, NULL, NULL)";
                command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                command.Parameters.Add(CreateParameter(command, "@dt_movimento", request.CreatedDateTime));
                command.Parameters.Add(CreateParameter(command, "@observacao", string.IsNullOrWhiteSpace(request.Observation) ? (object)DBNull.Value : request.Observation));
                command.Parameters.Add(CreateParameter(command, "@usuario", request.ActorUserName));
                command.Parameters.Add(CreateParameter(command, "@dt_hr_criacao", request.CreatedDateTime));
                command.Parameters.Add(CreateParameter(command, "@dt_hr_alteracao", NowText()));
                command.Parameters.Add(CreateParameter(command, "@dt_inicio", string.IsNullOrWhiteSpace(request.ScheduledDateTime) ? (object)DBNull.Value : request.ScheduledDateTime));
                command.Parameters.Add(CreateParameter(command, "@max_pontos", request.MaxOpenPoints));
                command.ExecuteNonQuery();
            }
        }

        private static void ReplaceInventoryItems(DbConnection connection, DbTransaction transaction, SaveInventoryRequest request)
        {
            using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = "DELETE FROM inventarios_itens WHERE numero = @numero";
                deleteCommand.Parameters.Add(CreateParameter(deleteCommand, "@numero", request.Number));
                deleteCommand.ExecuteNonQuery();
            }

            foreach (var item in request.Items)
            {
                var balance = GetStockBalanceAtInternal(connection, transaction, item.WarehouseCode, item.MaterialCode, item.LotCode, request.CreatedDateTime);
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO inventarios_itens
                            (numero, material, lote, almoxarifado, saldo_sistema, quantidade_contada, ajuste, tipo_ajuste, status, versao, dt_hr_criacao, dt_hr_alteracao)
                        VALUES
                            (@numero, @material, @lote, @almoxarifado, @saldo_sistema, NULL, NULL, NULL, 'ATIVO', 1, @agora, @agora)";
                    command.Parameters.Add(CreateParameter(command, "@numero", request.Number));
                    command.Parameters.Add(CreateParameter(command, "@material", item.MaterialCode));
                    command.Parameters.Add(CreateParameter(command, "@lote", item.LotCode));
                    command.Parameters.Add(CreateParameter(command, "@almoxarifado", item.WarehouseCode));
                    command.Parameters.Add(CreateParameter(command, "@saldo_sistema", balance));
                    command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ReplaceInventoryPoints(DbConnection connection, DbTransaction transaction, SaveInventoryRequest request)
        {
            using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = "DELETE FROM inventario_pontos WHERE numero = @numero";
                deleteCommand.Parameters.Add(CreateParameter(deleteCommand, "@numero", request.Number));
                deleteCommand.ExecuteNonQuery();
            }

            foreach (var point in request.Points)
            {
                InsertPoint(connection, transaction, request.Number, point, request.ActorUserName, point.Status);
            }
        }

        private static int InsertPoint(DbConnection connection, DbTransaction transaction, string inventoryNumber, InventoryPointInput point, string actorUserName, string status)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO inventario_pontos
                        (numero, nome_ponto, ip_ponto, computador, usuario_abertura, status, dt_abertura, dt_heartbeat)
                    VALUES
                        (@numero, @nome_ponto, @ip_ponto, @computador, @usuario_abertura, @status, @dt_abertura, @dt_heartbeat)
                    RETURNING id";
                command.Parameters.Add(CreateParameter(command, "@numero", inventoryNumber));
                command.Parameters.Add(CreateParameter(command, "@nome_ponto", point.PointName));
                command.Parameters.Add(CreateParameter(command, "@ip_ponto", string.IsNullOrWhiteSpace(point.IpAddress) ? (object)DBNull.Value : point.IpAddress));
                command.Parameters.Add(CreateParameter(command, "@computador", string.IsNullOrWhiteSpace(point.ComputerName) ? (object)DBNull.Value : point.ComputerName));
                command.Parameters.Add(CreateParameter(command, "@usuario_abertura", actorUserName));
                command.Parameters.Add(CreateParameter(command, "@status", string.IsNullOrWhiteSpace(status) ? "ABERTO" : status));
                command.Parameters.Add(CreateParameter(command, "@dt_abertura", NowText()));
                command.Parameters.Add(CreateParameter(command, "@dt_heartbeat", NowText()));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0);
            }
        }

        private static void EnsurePointAvailability(DbConnection connection, DbTransaction transaction, string inventoryNumber, int maxOpenPoints, string ipAddress, int? ignoredPointId)
        {
            using (var countCommand = connection.CreateCommand())
            {
                countCommand.Transaction = transaction;
                countCommand.CommandText = @"
                    SELECT COUNT(*)
                    FROM inventario_pontos
                    WHERE numero = @numero
                      AND status = 'ABERTO'
                      AND (@ignored_id IS NULL OR id <> @ignored_id)";
                countCommand.Parameters.Add(CreateParameter(countCommand, "@numero", inventoryNumber));
                countCommand.Parameters.Add(CreateParameter(countCommand, "@ignored_id", ignoredPointId.HasValue ? (object)ignoredPointId.Value : DBNull.Value));
                var openCount = Convert.ToInt32(countCommand.ExecuteScalar() ?? 0);
                if (openCount >= maxOpenPoints)
                {
                    throw new InvalidOperationException("Limite de pontos abertos atingido (" + openCount + "/" + maxOpenPoints + ").");
                }
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return;
            }

            using (var duplicateCommand = connection.CreateCommand())
            {
                duplicateCommand.Transaction = transaction;
                duplicateCommand.CommandText = @"
                    SELECT COUNT(*)
                    FROM inventario_pontos
                    WHERE numero = @numero
                      AND status = 'ABERTO'
                      AND COALESCE(TRIM(ip_ponto), '') = COALESCE(TRIM(@ip), '')
                      AND (@ignored_id IS NULL OR id <> @ignored_id)";
                duplicateCommand.Parameters.Add(CreateParameter(duplicateCommand, "@numero", inventoryNumber));
                duplicateCommand.Parameters.Add(CreateParameter(duplicateCommand, "@ip", ipAddress));
                duplicateCommand.Parameters.Add(CreateParameter(duplicateCommand, "@ignored_id", ignoredPointId.HasValue ? (object)ignoredPointId.Value : DBNull.Value));
                if (Convert.ToInt32(duplicateCommand.ExecuteScalar() ?? 0) > 0)
                {
                    throw new InvalidOperationException("Nao e permitido abrir dois pontos no mesmo IP neste inventario.");
                }
            }
        }
    }
}
