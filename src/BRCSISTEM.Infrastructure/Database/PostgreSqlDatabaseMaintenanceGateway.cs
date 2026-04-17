using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlDatabaseMaintenanceGateway : IDatabaseMaintenanceGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlDatabaseMaintenanceGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ── Audit log ──────────────────────────────────────────────────────────

        public IReadOnlyCollection<AuditLogEntry> LoadAuditLog(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText,
            int pageSize,
            int offset)
        {
            var results = new List<AuditLogEntry>();
            var where = BuildAuditWhereClause(filterUser, filterAction, filterDateFrom, filterDateTo, searchText);
            var dateExpression = BuildAuditDateExpression();

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT id, dt_hr, usuario, acao, detalhes
                    FROM logs_auditoria
                    {where}
                    ORDER BY {dateExpression} DESC NULLS LAST, dt_hr DESC, id DESC
                    LIMIT @pageSize OFFSET @offset";

                AddParameter(command, "pageSize", pageSize);
                AddParameter(command, "offset", offset);
                AddAuditFilterParameters(command, filterUser, filterAction, filterDateFrom, filterDateTo, searchText);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new AuditLogEntry
                        {
                            Id = ReadLong(reader, "id"),
                            DateTime = ReadString(reader, "dt_hr"),
                            UserName = ReadString(reader, "usuario"),
                            Action = ReadString(reader, "acao"),
                            Details = ReadString(reader, "detalhes"),
                        });
                    }
                }
            }

            return results;
        }

        public int CountAuditLog(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText)
        {
            var where = BuildAuditWhereClause(filterUser, filterAction, filterDateFrom, filterDateTo, searchText);
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM logs_auditoria {where}";
                AddAuditFilterParameters(command, filterUser, filterAction, filterDateFrom, filterDateTo, searchText);
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public IReadOnlyCollection<string> LoadAuditUsers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<string>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT usuario
                    FROM logs_auditoria
                    WHERE usuario IS NOT NULL
                      AND TRIM(usuario) <> ''
                    ORDER BY usuario";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(ReadString(reader, "usuario"));
                    }
                }
            }

            return results;
        }

        public IReadOnlyCollection<string> LoadAuditActions(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<string>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT acao
                    FROM logs_auditoria
                    WHERE acao IS NOT NULL
                      AND TRIM(acao) <> ''
                    ORDER BY acao";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(ReadString(reader, "acao"));
                    }
                }
            }

            return results;
        }

        private static string BuildAuditWhereClause(string filterUser, string filterAction, string filterDateFrom, string filterDateTo, string searchText)
        {
            var conditions = new List<string>();
            var dateExpression = BuildAuditDateExpression();
            if (!string.IsNullOrWhiteSpace(filterUser)) conditions.Add("usuario = @filterUser");
            if (!string.IsNullOrWhiteSpace(filterAction)) conditions.Add("acao = @filterAction");
            if (!string.IsNullOrWhiteSpace(filterDateFrom)) conditions.Add(dateExpression + " >= to_timestamp(@filterDateFrom, 'YYYY-MM-DD HH24:MI:SS')");
            if (!string.IsNullOrWhiteSpace(filterDateTo)) conditions.Add(dateExpression + " < to_timestamp(@filterDateTo, 'YYYY-MM-DD HH24:MI:SS')");
            if (!string.IsNullOrWhiteSpace(searchText)) conditions.Add("(usuario ILIKE @searchText OR acao ILIKE @searchText OR detalhes ILIKE @searchText)");
            return conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
        }

        private static void AddAuditFilterParameters(DbCommand command, string filterUser, string filterAction, string filterDateFrom, string filterDateTo, string searchText)
        {
            if (!string.IsNullOrWhiteSpace(filterUser)) AddParameter(command, "filterUser", filterUser.Trim());
            if (!string.IsNullOrWhiteSpace(filterAction)) AddParameter(command, "filterAction", filterAction.Trim());
            if (!string.IsNullOrWhiteSpace(filterDateFrom)) AddParameter(command, "filterDateFrom", filterDateFrom.Trim());
            if (!string.IsNullOrWhiteSpace(filterDateTo)) AddParameter(command, "filterDateTo", filterDateTo.Trim());
            if (!string.IsNullOrWhiteSpace(searchText)) AddParameter(command, "searchText", "%" + searchText.Trim() + "%");
        }

        private static string BuildAuditDateExpression()
        {
            return "CASE "
                 + "WHEN dt_hr ~ '^[0-9]{2}/[0-9]{2}/[0-9]{4}' THEN to_timestamp(dt_hr, 'DD/MM/YYYY HH24:MI:SS') "
                 + "WHEN dt_hr ~ '^[0-9]{4}-[0-9]{2}-[0-9]{2}' THEN to_timestamp(dt_hr, 'YYYY-MM-DD HH24:MI:SS') "
                 + "ELSE NULL END";
        }

        // ── System parameters ──────────────────────────────────────────────────

        public IReadOnlyCollection<SystemParameter> LoadSystemParameters(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<SystemParameter>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT chave, valor FROM parametros ORDER BY chave";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new SystemParameter
                        {
                            Key = ReadString(reader, "chave"),
                            Value = ReadString(reader, "valor"),
                        });
                    }
                }
            }

            return results;
        }

        public void SaveSystemParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string value)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO parametros (chave, valor, dt_hr_alteracao)
                    VALUES (@key, @value, @now)
                    ON CONFLICT (chave) DO UPDATE
                    SET valor = EXCLUDED.valor,
                        dt_hr_alteracao = EXCLUDED.dt_hr_alteracao";

                AddParameter(command, "key", key);
                AddParameter(command, "value", value ?? string.Empty);
                AddParameter(command, "now", NowText());
                command.ExecuteNonQuery();
            }
        }

        // ── Shifts (turnos) ────────────────────────────────────────────────────

        public IReadOnlyCollection<ShiftSummary> LoadShifts(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<ShiftSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nome, descricao, ativo FROM turnos ORDER BY nome";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new ShiftSummary
                        {
                            Id = ReadInt(reader, "id"),
                            Name = ReadString(reader, "nome"),
                            Description = ReadString(reader, "descricao"),
                            IsActive = ReadBool(reader, "ativo"),
                        });
                    }
                }
            }

            return results;
        }

        public void AddShift(DatabaseProfile profile, ConnectionResilienceSettings settings, string name, string description)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO turnos (nome, descricao, ativo) VALUES (@nome, @descricao, true)";
                AddParameter(command, "nome", name.Trim());
                AddParameter(command, "descricao", (description ?? string.Empty).Trim());
                command.ExecuteNonQuery();
            }
        }

        public void UpdateShift(DatabaseProfile profile, ConnectionResilienceSettings settings, int id, string name, string description)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE turnos SET nome = @nome, descricao = @descricao WHERE id = @id";
                AddParameter(command, "nome", name.Trim());
                AddParameter(command, "descricao", (description ?? string.Empty).Trim());
                AddParameter(command, "id", id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteShift(DatabaseProfile profile, ConnectionResilienceSettings settings, int id)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM turnos WHERE id = @id";
                AddParameter(command, "id", id);
                command.ExecuteNonQuery();
            }
        }

        // ── Requisition reasons ────────────────────────────────────────────────

        public IReadOnlyCollection<RequisitionReasonSummary> LoadRequisitionReasons(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<RequisitionReasonSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nome, descricao, ativo FROM motivos_requisicao ORDER BY nome";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new RequisitionReasonSummary
                        {
                            Id = ReadInt(reader, "id"),
                            Name = ReadString(reader, "nome"),
                            Description = ReadString(reader, "descricao"),
                            IsActive = ReadBool(reader, "ativo"),
                        });
                    }
                }
            }

            return results;
        }

        public void AddRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, string name, string description)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO motivos_requisicao (nome, descricao, ativo) VALUES (@nome, @descricao, true)";
                AddParameter(command, "nome", name.Trim());
                AddParameter(command, "descricao", (description ?? string.Empty).Trim());
                command.ExecuteNonQuery();
            }
        }

        public void UpdateRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, int id, string name, string description)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE motivos_requisicao SET nome = @nome, descricao = @descricao WHERE id = @id";
                AddParameter(command, "nome", name.Trim());
                AddParameter(command, "descricao", (description ?? string.Empty).Trim());
                AddParameter(command, "id", id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, int id)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM motivos_requisicao WHERE id = @id";
                AddParameter(command, "id", id);
                command.ExecuteNonQuery();
            }
        }

        // ── Warehouse access ───────────────────────────────────────────────────

        public IReadOnlyCollection<WarehouseAccessEntry> LoadGrantedWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName)
        {
            var results = new List<WarehouseAccessEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT a.codigo, a.nome
                    FROM parametros_almoxarifados pa
                    INNER JOIN almoxarifados a ON pa.almoxarifado = a.codigo
                    WHERE pa.usuario = @usuario
                    ORDER BY a.nome";

                AddParameter(command, "usuario", userName.Trim());
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new WarehouseAccessEntry
                        {
                            WarehouseCode = ReadString(reader, "codigo"),
                            WarehouseName = ReadString(reader, "nome"),
                        });
                    }
                }
            }

            return results;
        }

        public IReadOnlyCollection<WarehouseAccessEntry> LoadAvailableWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName)
        {
            var results = new List<WarehouseAccessEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT a.codigo, a.nome
                    FROM almoxarifados a
                    WHERE a.ativo = true
                      AND a.codigo NOT IN (
                          SELECT almoxarifado FROM parametros_almoxarifados WHERE usuario = @usuario
                      )
                    ORDER BY a.nome";

                AddParameter(command, "usuario", userName.Trim());
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new WarehouseAccessEntry
                        {
                            WarehouseCode = ReadString(reader, "codigo"),
                            WarehouseName = ReadString(reader, "nome"),
                        });
                    }
                }
            }

            return results;
        }

        public void GrantWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName, string warehouseCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO parametros_almoxarifados (usuario, almoxarifado)
                    VALUES (@usuario, @almoxarifado)
                    ON CONFLICT DO NOTHING";

                AddParameter(command, "usuario", userName.Trim());
                AddParameter(command, "almoxarifado", warehouseCode.Trim());
                command.ExecuteNonQuery();
            }
        }

        public void RevokeWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName, string warehouseCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM parametros_almoxarifados WHERE usuario = @usuario AND almoxarifado = @almoxarifado";
                AddParameter(command, "usuario", userName.Trim());
                AddParameter(command, "almoxarifado", warehouseCode.Trim());
                command.ExecuteNonQuery();
            }
        }

        // ── Locked records ─────────────────────────────────────────────────────

        public IReadOnlyCollection<OpenMovementLockSummary> LoadLockedRecords(DatabaseProfile profile, ConnectionResilienceSettings settings, string tableName, string documentNumber, string supplier)
        {
            var results = new List<OpenMovementLockSummary>();

            var tables = new[] { "notas", "transferencias", "saidas_producao", "requisicoes" };
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tables = new[] { tableName.Trim().ToLowerInvariant() };
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            {
                foreach (var table in tables)
                {
                    string numberColumn, supplierColumn;
                    GetDocumentColumns(table, out numberColumn, out supplierColumn);

                    var sb = new StringBuilder();
                    if (supplierColumn != null)
                    {
                        sb.Append($"SELECT '{table}' AS tipo, {numberColumn} AS numero, {supplierColumn} AS fornecedor, bloqueado_por, bloqueado_em FROM {table} WHERE bloqueado_por IS NOT NULL");
                    }
                    else
                    {
                        sb.Append($"SELECT '{table}' AS tipo, {numberColumn} AS numero, ''::text AS fornecedor, bloqueado_por, bloqueado_em FROM {table} WHERE bloqueado_por IS NOT NULL");
                    }

                    if (!string.IsNullOrWhiteSpace(documentNumber))
                        sb.Append($" AND LOWER({numberColumn}::text) LIKE @docNum");
                    if (!string.IsNullOrWhiteSpace(supplier) && supplierColumn != null)
                        sb.Append($" AND LOWER({supplierColumn}::text) LIKE @supplier");

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sb.ToString();
                        if (!string.IsNullOrWhiteSpace(documentNumber))
                            AddParameter(command, "docNum", "%" + documentNumber.Trim().ToLowerInvariant() + "%");
                        if (!string.IsNullOrWhiteSpace(supplier) && supplierColumn != null)
                            AddParameter(command, "supplier", "%" + supplier.Trim().ToLowerInvariant() + "%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new OpenMovementLockSummary
                                {
                                    Type = ReadString(reader, "tipo"),
                                    DocumentNumber = ReadString(reader, "numero"),
                                    Supplier = ReadString(reader, "fornecedor"),
                                    UserName = ReadString(reader, "bloqueado_por"),
                                    LockedAt = ReadString(reader, "bloqueado_em"),
                                });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public void UnlockRecord(DatabaseProfile profile, ConnectionResilienceSettings settings, string tableName, string documentNumber, string supplier)
        {
            string numberColumn, supplierColumn;
            GetDocumentColumns(tableName, out numberColumn, out supplierColumn);

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var sb = new StringBuilder();
                sb.Append($"UPDATE {tableName} SET bloqueado_por = NULL, bloqueado_em = NULL WHERE {numberColumn} = @docNum");

                if (!string.IsNullOrWhiteSpace(supplier) && supplierColumn != null)
                    sb.Append($" AND {supplierColumn} = @supplier");

                command.CommandText = sb.ToString();
                AddParameter(command, "docNum", documentNumber.Trim());
                if (!string.IsNullOrWhiteSpace(supplier) && supplierColumn != null)
                    AddParameter(command, "supplier", supplier.Trim());

                command.ExecuteNonQuery();
            }
        }

        private static void GetDocumentColumns(string tableName, out string numberColumn, out string supplierColumn)
        {
            switch (tableName.ToLowerInvariant())
            {
                case "notas":
                    numberColumn = "numero";
                    supplierColumn = "fornecedor";
                    break;
                case "transferencias":
                    numberColumn = "numero";
                    supplierColumn = null;
                    break;
                case "saidas_producao":
                    numberColumn = "numero";
                    supplierColumn = null;
                    break;
                case "requisicoes":
                    numberColumn = "numero";
                    supplierColumn = null;
                    break;
                default:
                    numberColumn = "numero";
                    supplierColumn = null;
                    break;
            }
        }

        // ── Remove note ────────────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadNoteHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, fornecedor, almoxarifado, dt_emissao, dt_movimento, status, usuario, bloqueado_por
                    FROM notas
                    WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup
                    LIMIT 1";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());
                AddParameter(command, "sup", supplier.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DocumentMaintenanceHeader
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            DocumentType = "NOTA",
                            Warehouse = ReadString(reader, "almoxarifado"),
                            EmissionDate = ReadString(reader, "dt_emissao"),
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            UserName = ReadString(reader, "usuario"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                        };
                    }
                }
            }

            return null;
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadNoteItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier)
        {
            var results = new List<DocumentMaintenanceItem>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT ni.material, e.nome AS material_nome, ni.lote, ni.quantidade, ni.unidade, ni.almoxarifado
                    FROM notas_itens ni
                    LEFT JOIN embalagens e ON ni.material = e.codigo
                    WHERE LOWER(ni.numero) = @num AND LOWER(ni.fornecedor) = @sup
                      AND ni.status = 'ATIVO'
                    ORDER BY ni.material, ni.lote";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());
                AddParameter(command, "sup", supplier.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentMaintenanceItem
                        {
                            Material = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            Unit = ReadString(reader, "unidade"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                        });
                    }
                }
            }

            return results;
        }

        public RemoveNoteResult RemoveNote(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                var normalizedNumber = number.Trim().ToLowerInvariant();
                var normalizedSupplier = supplier.Trim().ToLowerInvariant();

                int removedMovements;
                int removedItems;
                int removedNotes;

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'NOTA' AND LOWER(fornecedor) = @sup";
                    AddParameter(command, "num", normalizedNumber);
                    AddParameter(command, "sup", normalizedSupplier);
                    removedMovements = command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "DELETE FROM notas_itens WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup";
                    AddParameter(command, "num", normalizedNumber);
                    AddParameter(command, "sup", normalizedSupplier);
                    removedItems = command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "DELETE FROM notas WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup";
                    AddParameter(command, "num", normalizedNumber);
                    AddParameter(command, "sup", normalizedSupplier);
                    removedNotes = command.ExecuteNonQuery();
                }

                if (removedNotes == 0)
                {
                    throw new InvalidOperationException("Nota nao encontrada para remocao.");
                }

                transaction.Commit();
                return new RemoveNoteResult
                {
                    Number = number,
                    Supplier = supplier,
                    RemovedItems = removedItems,
                    RemovedMovements = removedMovements,
                };
            }
        }

        public IReadOnlyCollection<InboundReceiptReactivationEntry> SearchCancelledInboundReceipts(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, int limit)
        {
            var results = new List<InboundReceiptReactivationEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var conditions = new List<string> { "status = 'CANCELADA'" };
                if (!string.IsNullOrWhiteSpace(number))
                {
                    conditions.Add("numero = @numero");
                    AddParameter(command, "numero", number.Trim());
                }

                if (!string.IsNullOrWhiteSpace(supplier))
                {
                    conditions.Add("fornecedor = @fornecedor");
                    AddParameter(command, "fornecedor", supplier.Trim());
                }

                var sql = new StringBuilder();
                sql.AppendLine("SELECT numero, fornecedor, almoxarifado, versao, dt_emissao, status");
                sql.AppendLine("FROM notas");
                sql.AppendLine("WHERE " + string.Join(" AND ", conditions));
                sql.AppendLine("ORDER BY dt_emissao DESC");
                if (limit > 0)
                {
                    sql.AppendLine("LIMIT @limit");
                    AddParameter(command, "limit", limit);
                }

                command.CommandText = sql.ToString();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new InboundReceiptReactivationEntry
                        {
                            Number = ReadString(reader, "numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            Version = ReadInt(reader, "versao"),
                            EmissionDate = ReadString(reader, "dt_emissao"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return results;
        }

        public void ReactivateInboundReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, int version)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                var current = LoadInboundReceiptForReactivation(connection, transaction, number, supplier, version);
                if (current == null)
                {
                    throw new InvalidOperationException("A nota selecionada nao esta disponivel para reativacao.");
                }

                var nowText = NowText();

                ExecuteNonQuery(
                    connection,
                    transaction,
                    @"UPDATE notas
                      SET status = 'ATIVO',
                          bloqueado_por = NULL,
                          bloqueado_em = NULL,
                          dt_hr_alteracao = @now
                      WHERE numero = @numero AND fornecedor = @fornecedor AND versao = @versao",
                    ("now", nowText),
                    ("numero", number),
                    ("fornecedor", supplier),
                    ("versao", version));

                ExecuteNonQuery(
                    connection,
                    transaction,
                    @"UPDATE notas_itens
                      SET status = 'ATIVO',
                          dt_hr_alteracao = @now
                      WHERE numero = @numero AND versao = @versao",
                    ("now", nowText),
                    ("numero", number),
                    ("versao", version));

                ExecuteNonQuery(
                    connection,
                    transaction,
                    @"UPDATE movimentos_estoque
                      SET status = 'ATIVO',
                          dt_hr_alteracao = @now
                      WHERE documento_numero = @numero
                        AND documento_tipo = 'NOTA'
                        AND fornecedor = @fornecedor",
                    ("now", nowText),
                    ("numero", number),
                    ("fornecedor", supplier));

                transaction.Commit();
            }
        }

        // ── Remove transfer ────────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadTransferHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, dt_movimento, status, bloqueado_por
                    FROM transferencias
                    WHERE LOWER(numero) = @num
                    LIMIT 1";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DocumentMaintenanceHeader
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            DocumentType = "TRANSFERENCIA",
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                        };
                    }
                }
            }

            return null;
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadTransferItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            var results = new List<DocumentMaintenanceItem>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT ti.material, e.nome AS material_nome, ti.lote, ti.quantidade, ti.unidade,
                           ti.almoxarifado_origem AS almoxarifado
                    FROM transferencias_itens ti
                    LEFT JOIN embalagens e ON ti.material = e.codigo
                    WHERE LOWER(ti.numero) = @num
                    ORDER BY ti.material, ti.lote";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentMaintenanceItem
                        {
                            Material = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            Unit = ReadString(reader, "unidade"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                        });
                    }
                }
            }

            return results;
        }

        public void RemoveTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'TRANSFERENCIA'",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM transferencias_itens WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM transferencias WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Remove production output ───────────────────────────────────────────

        public DocumentMaintenanceHeader LoadProductionOutputHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, dt_movimento, status, bloqueado_por
                    FROM saidas_producao
                    WHERE LOWER(numero) = @num
                    LIMIT 1";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DocumentMaintenanceHeader
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            DocumentType = "SAIDA_PRODUCAO",
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                        };
                    }
                }
            }

            return null;
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadProductionOutputItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            var results = new List<DocumentMaintenanceItem>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT si.material, e.nome AS material_nome, si.lote, si.quantidade, si.unidade, si.almoxarifado
                    FROM saidas_producao_itens si
                    LEFT JOIN embalagens e ON si.material = e.codigo
                    WHERE LOWER(si.numero) = @num
                    ORDER BY si.material, si.lote";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentMaintenanceItem
                        {
                            Material = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            Unit = ReadString(reader, "unidade"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                        });
                    }
                }
            }

            return results;
        }

        public void RemoveProductionOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'SAIDA_PRODUCAO'",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM saidas_producao_itens WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM saidas_producao WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Remove requisition ─────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadRequisitionHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, dt_movimento, almoxarifado, status
                    FROM requisicoes
                    WHERE LOWER(numero) = @num
                    LIMIT 1";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        {
                            return new DocumentMaintenanceHeader
                            {
                                DocumentNumber = ReadString(reader, "numero"),
                                DocumentType = "REQUISICAO",
                                Date = ReadString(reader, "dt_movimento"),
                                Warehouse = ReadString(reader, "almoxarifado"),
                                Status = ReadString(reader, "status"),
                                UserName = string.Empty,
                            };
                        }
                    }
            }

            return null;
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadRequisitionItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            var results = new List<DocumentMaintenanceItem>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT ri.material, ri.lote, ri.almoxarifado, ri.quantidade
                    FROM requisicoes_itens ri
                    WHERE LOWER(ri.numero) = @num
                      AND ri.status = 'ATIVO'
                    ORDER BY ri.material, ri.lote";

                AddParameter(command, "num", number.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        {
                            results.Add(new DocumentMaintenanceItem
                            {
                                Material = ReadString(reader, "material"),
                                Lot = ReadString(reader, "lote"),
                                Quantity = ReadDecimal(reader, "quantidade"),
                                Warehouse = ReadString(reader, "almoxarifado"),
                            });
                        }
                }
            }

            return results;
        }

        public RemoveRequisitionResult RemoveRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                var normalizedNumber = (number ?? string.Empty).Trim().ToLowerInvariant();
                var result = new RemoveRequisitionResult
                {
                    Number = (number ?? string.Empty).Trim(),
                };

                using (var deleteMovements = connection.CreateCommand())
                {
                    deleteMovements.Transaction = transaction;
                    deleteMovements.CommandText = "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'REQUISICAO'";
                    AddParameter(deleteMovements, "num", normalizedNumber);
                    result.RemovedMovements = deleteMovements.ExecuteNonQuery();
                }

                using (var deleteItems = connection.CreateCommand())
                {
                    deleteItems.Transaction = transaction;
                    deleteItems.CommandText = "DELETE FROM requisicoes_itens WHERE LOWER(numero) = @num";
                    AddParameter(deleteItems, "num", normalizedNumber);
                    result.RemovedItems = deleteItems.ExecuteNonQuery();
                }

                using (var deleteHeader = connection.CreateCommand())
                {
                    deleteHeader.Transaction = transaction;
                    deleteHeader.CommandText = "DELETE FROM requisicoes WHERE LOWER(numero) = @num";
                    AddParameter(deleteHeader, "num", normalizedNumber);
                    deleteHeader.ExecuteNonQuery();
                }

                transaction.Commit();
                return result;
            }
        }

        // ── Change note date ───────────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveNotes(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            // Espelha views/bd_alterar_data_entrada.py::_carregar_notas +
            // _exibir_dados_nota em uma unica consulta (Python faz N+1 a cada selecao).
            const string sql = @"
                SELECT n.numero,
                       n.fornecedor,
                       COALESCE(f.nome, '')      AS fornecedor_nome,
                       n.dt_movimento,
                       n.status,
                       n.almoxarifado,
                       COALESCE(a.nome, '')      AS almoxarifado_nome,
                       COALESCE(i.qtd, 0)        AS qtd_itens
                FROM notas n
                LEFT JOIN fornecedores  f ON f.codigo = n.fornecedor    AND f.status = 'ATIVO'
                LEFT JOIN almoxarifados a ON a.codigo = n.almoxarifado  AND a.status = 'ATIVO'
                LEFT JOIN (
                    SELECT numero, fornecedor, COUNT(material) AS qtd
                    FROM notas_itens
                    GROUP BY numero, fornecedor
                ) i ON i.numero = n.numero AND i.fornecedor = n.fornecedor
                WHERE n.status = 'ATIVO'
                ORDER BY n.dt_movimento DESC, n.numero";

            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentDateEntry
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Supplier       = ReadString(reader, "fornecedor"),
                            SupplierName   = ReadString(reader, "fornecedor_nome"),
                            Date           = ReadString(reader, "dt_movimento"),
                            Status         = ReadString(reader, "status"),
                            Warehouse      = ReadString(reader, "almoxarifado"),
                            WarehouseName  = ReadString(reader, "almoxarifado_nome"),
                            ItemCount      = ReadInt(reader, "qtd_itens"),
                        });
                    }
                }
            }

            return results;
        }

        public ChangeDateResult ChangeNoteDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, string newDate)
        {
            // Fidelidade a views/bd_alterar_data_entrada.py::_alterar:
            //   SELECT numero, fornecedor FROM notas WHERE numero = %s   (SEM filtro por fornecedor)
            //   UPDATE notas SET dt_movimento=ISO, dt_emissao=BR WHERE numero = %s
            //   UPDATE movimentos_estoque SET data_movimento=ISO WHERE documento_tipo='NOTA' AND documento_numero = %s
            // `newDate` chega ja em ISO (yyyy-MM-dd HH:mm:ss); `supplier` e opcional
            // (Python obtem do proprio SELECT). Usamos ::timestamp para preservar hora.
            var num = (number ?? string.Empty).Trim();
            var iso = (newDate ?? string.Empty).Trim();
            var br  = IsoToBrazilian(iso);

            var result = new ChangeDateResult();

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                // 1. Localiza nota e captura fornecedor
                using (var lookup = connection.CreateCommand())
                {
                    lookup.Transaction = transaction;
                    lookup.CommandText = "SELECT numero, fornecedor FROM notas WHERE LOWER(numero) = @num LIMIT 1";
                    AddParameter(lookup, "num", num.ToLowerInvariant());
                    using (var reader = lookup.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            transaction.Rollback();
                            throw new InvalidOperationException("Nota " + num + " nao encontrada.");
                        }
                        result.Supplier = ReadString(reader, "fornecedor");
                    }
                }

                // 2. UPDATE notas (dt_movimento ISO, dt_emissao BR)
                using (var upd = connection.CreateCommand())
                {
                    upd.Transaction = transaction;
                    upd.CommandText = "UPDATE notas SET dt_movimento = @iso::timestamp, dt_emissao = @br WHERE LOWER(numero) = @num";
                    AddParameter(upd, "iso", iso);
                    AddParameter(upd, "br",  br);
                    AddParameter(upd, "num", num.ToLowerInvariant());
                    result.HeaderRowsUpdated = upd.ExecuteNonQuery();
                }

                // 3. UPDATE movimentos_estoque
                using (var upd = connection.CreateCommand())
                {
                    upd.Transaction = transaction;
                    upd.CommandText = "UPDATE movimentos_estoque SET data_movimento = @iso::timestamp WHERE documento_tipo = 'NOTA' AND LOWER(documento_numero) = @num";
                    AddParameter(upd, "iso", iso);
                    AddParameter(upd, "num", num.ToLowerInvariant());
                    result.MovementRowsUpdated = upd.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return result;
        }

        private static string IsoToBrazilian(string iso)
        {
            if (string.IsNullOrWhiteSpace(iso)) return iso;
            DateTime dt;
            if (DateTime.TryParseExact(iso, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            if (DateTime.TryParseExact(iso, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return iso;
        }

        // ── Change transfer date ───────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveTransfers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT t.numero,
                           t.dt_movimento,
                           t.status,
                           COALESCE(t.almox_origem, '') AS almox_origem,
                           COALESCE(ao.nome, '') AS almox_origem_nome,
                           COALESCE(t.almox_destino, '') AS almox_destino,
                           COALESCE(ad.nome, '') AS almox_destino_nome,
                           COALESCE(i.qtd_itens, 0) AS qtd_itens
                    FROM transferencias t
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM transferencias
                        GROUP BY numero
                    ) tx ON tx.numero = t.numero AND tx.max_versao = t.versao
                    LEFT JOIN almoxarifados ao ON ao.codigo = t.almox_origem
                        AND ao.versao = (
                            SELECT MAX(x.versao)
                            FROM almoxarifados x
                            WHERE x.codigo = ao.codigo
                        )
                    LEFT JOIN almoxarifados ad ON ad.codigo = t.almox_destino
                        AND ad.versao = (
                            SELECT MAX(x.versao)
                            FROM almoxarifados x
                            WHERE x.codigo = ad.codigo
                        )
                    LEFT JOIN (
                        SELECT numero, versao, COUNT(material) AS qtd_itens
                        FROM transferencias_itens
                        GROUP BY numero, versao
                    ) i ON i.numero = t.numero AND i.versao = t.versao
                    WHERE t.status = 'ATIVO'
                    ORDER BY t.dt_movimento DESC, t.numero DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentDateEntry
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            OriginWarehouse = ReadString(reader, "almox_origem"),
                            OriginWarehouseName = ReadString(reader, "almox_origem_nome"),
                            DestinationWarehouse = ReadString(reader, "almox_destino"),
                            DestinationWarehouseName = ReadString(reader, "almox_destino_nome"),
                            ItemCount = ReadInt(reader, "qtd_itens"),
                        });
                    }
                }
            }

            return results;
        }

        public ChangeDateResult ChangeTransferDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate)
        {
            var result = new ChangeDateResult();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                var normalizedNumber = (number ?? string.Empty).Trim().ToLowerInvariant();
                var normalizedDate = (newDate ?? string.Empty).Trim();

                using (var verify = connection.CreateCommand())
                {
                    verify.Transaction = transaction;
                    verify.CommandText = @"
                        SELECT numero
                        FROM transferencias
                        WHERE LOWER(numero) = @num
                          AND status = 'ATIVO'
                        LIMIT 1";
                    AddParameter(verify, "num", normalizedNumber);
                    var exists = verify.ExecuteScalar();
                    if (exists == null || exists == DBNull.Value)
                    {
                        throw new InvalidOperationException("Transferencia " + number + " nao encontrada!");
                    }
                }

                using (var updateTransfer = connection.CreateCommand())
                {
                    updateTransfer.Transaction = transaction;
                    updateTransfer.CommandText = @"
                        UPDATE transferencias
                        SET dt_movimento = @newDate
                        WHERE LOWER(numero) = @num
                          AND status = 'ATIVO'";
                    AddParameter(updateTransfer, "newDate", normalizedDate);
                    AddParameter(updateTransfer, "num", normalizedNumber);
                    result.HeaderRowsUpdated = updateTransfer.ExecuteNonQuery();
                }

                using (var updateMovements = connection.CreateCommand())
                {
                    updateMovements.Transaction = transaction;
                    updateMovements.CommandText = @"
                        UPDATE movimentos_estoque
                        SET data_movimento = @newDate
                        WHERE documento_tipo = 'TRANSFERENCIA'
                          AND LOWER(documento_numero) = @num
                          AND status = 'ATIVO'";
                    AddParameter(updateMovements, "newDate", normalizedDate);
                    AddParameter(updateMovements, "num", normalizedNumber);
                    result.MovementRowsUpdated = updateMovements.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return result;
        }

        // ── Change production output date ──────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveProductionOutputs(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT sp.numero,
                           sp.dt_movimento,
                           sp.status,
                           COALESCE(i.almoxarifado, '') AS almoxarifado,
                           COALESCE(sp.turno, '') AS turno,
                           COALESCE(sp.finalidade, '') AS finalidade,
                           COALESCE(i.qtd_itens, 0) AS qtd_itens
                    FROM saidas_producao sp
                    LEFT JOIN (
                        SELECT numero,
                        MAX(almoxarifado) AS almoxarifado,
                        COUNT(material) AS qtd_itens
                        FROM saidas_producao_itens
                        WHERE status = 'ATIVO'
                        GROUP BY numero
                    ) i ON i.numero = sp.numero
                    WHERE sp.status = 'ATIVO'
                    ORDER BY sp.dt_movimento DESC, sp.numero";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentDateEntry
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            OriginWarehouse = ReadString(reader, "almoxarifado"),
                            Shift = ReadString(reader, "turno"),
                            Purpose = ReadString(reader, "finalidade"),
                            ItemCount = ReadInt(reader, "qtd_itens"),
                        });
                    }
                }
            }

            return results;
        }

        public ChangeDateResult ChangeProductionOutputDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate)
        {
            var result = new ChangeDateResult();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var lookup = connection.CreateCommand())
                {
                    lookup.Transaction = transaction;
                    lookup.CommandText = "SELECT numero FROM saidas_producao WHERE LOWER(numero) = @num AND status = 'ATIVO' LIMIT 1";
                    AddParameter(lookup, "num", number.Trim().ToLowerInvariant());
                    using (var reader = lookup.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            transaction.Rollback();
                            throw new InvalidOperationException("Saida " + number.Trim() + " nao encontrada.");
                        }
                    }
                }

                using (var updateHeader = connection.CreateCommand())
                {
                    updateHeader.Transaction = transaction;
                    updateHeader.CommandText = "UPDATE saidas_producao SET dt_movimento = @newDate::timestamp WHERE LOWER(numero) = @num AND status = 'ATIVO'";
                    AddParameter(updateHeader, "newDate", newDate.Trim());
                    AddParameter(updateHeader, "num", number.Trim().ToLowerInvariant());
                    result.HeaderRowsUpdated = updateHeader.ExecuteNonQuery();
                }

                using (var updateMovements = connection.CreateCommand())
                {
                    updateMovements.Transaction = transaction;
                    updateMovements.CommandText = "UPDATE movimentos_estoque SET data_movimento = @newDate::timestamp WHERE documento_tipo = 'SAIDA_PRODUCAO' AND LOWER(documento_numero) = @num AND status = 'ATIVO'";
                    AddParameter(updateMovements, "newDate", newDate.Trim());
                    AddParameter(updateMovements, "num", number.Trim().ToLowerInvariant());
                    result.MovementRowsUpdated = updateMovements.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return result;
        }

        // ── Alert: divergent lot entries ───────────────────────────────────────

        public IReadOnlyCollection<DivergentLotEntry> DiagnoseDivergentLotEntries(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DivergentLotEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                // SQL espelha exatamente diagnosticar_entradas_lote_divergente() do Python:
                // - JOIN notas_itens para obter o lote correto e quantidade esperada
                // - JOIN notas para obter o usuario que criou a nota
                // - NOT EXISTS confirma que o lote do movimento realmente não existe nos itens
                command.CommandText = @"
                    SELECT
                        me.id,
                        me.documento_numero,
                        me.material,
                        me.lote                AS lote_movimento,
                        me.fornecedor,
                        me.almoxarifado,
                        me.quantidade,
                        me.usuario             AS usuario_movimento,
                        me.dt_hr_criacao,
                        ni.lote                AS lote_notas_itens,
                        ni.quantidade          AS qtd_notas_itens,
                        n.usuario              AS usuario_nota,
                        e.descricao            AS material_descricao
                    FROM movimentos_estoque me
                    JOIN notas_itens ni
                      ON  ni.numero   = me.documento_numero
                      AND ni.material = me.material
                      AND ni.status   = 'ATIVO'
                    JOIN notas n
                      ON  n.numero  = me.documento_numero
                      AND n.status  = 'ATIVO'
                      AND n.versao  = (
                              SELECT MAX(nx.versao)
                              FROM notas nx
                              WHERE nx.numero = n.numero AND nx.status = 'ATIVO'
                          )
                    LEFT JOIN embalagens e
                      ON  e.codigo = me.material
                      AND e.versao = (
                              SELECT MAX(ex.versao)
                              FROM embalagens ex
                              WHERE ex.codigo = e.codigo
                          )
                    WHERE me.tipo           = 'ENTRADA'
                      AND me.documento_tipo = 'NOTA'
                      AND me.status         = 'ATIVO'
                      AND NOT EXISTS (
                          SELECT 1
                          FROM notas_itens ni2
                          WHERE ni2.numero   = me.documento_numero
                            AND ni2.material = me.material
                            AND ni2.lote     = me.lote
                            AND ni2.status   = 'ATIVO'
                      )
                    ORDER BY me.documento_numero, me.material, me.id";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DivergentLotEntry
                        {
                            MovementId        = ReadLong(reader,    "id"),
                            DocumentNumber    = ReadString(reader,  "documento_numero"),
                            Material          = ReadString(reader,  "material"),
                            MaterialName      = ReadString(reader,  "material_descricao"),
                            LotInMovement     = ReadString(reader,  "lote_movimento"),
                            LotInNoteItem     = ReadString(reader,  "lote_notas_itens"),
                            Supplier          = ReadString(reader,  "fornecedor"),
                            Warehouse         = ReadString(reader,  "almoxarifado"),
                            Quantity          = ReadDecimal(reader, "quantidade"),
                            QuantityInNoteItem = ReadDecimal(reader, "qtd_notas_itens"),
                            MovementUser      = ReadString(reader,  "usuario_movimento"),
                            NoteUser          = ReadString(reader,  "usuario_nota"),
                            CreatedAt         = ReadString(reader,  "dt_hr_criacao"),
                        });
                    }
                }
            }

            return results;
        }

        public void FixDivergentLotEntry(DatabaseProfile profile, ConnectionResilienceSettings settings, long movementId)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE movimentos_estoque
                    SET status = 'INATIVO', dt_hr_alteracao = @now
                    WHERE id = @id AND status = 'ATIVO'";

                AddParameter(command, "id", movementId);
                AddParameter(command, "now", NowText());
                command.ExecuteNonQuery();
            }
        }

        // ── Alert: negative stock ──────────────────────────────────────────────

        public IReadOnlyCollection<NegativeStockEntry> DiagnoseNegativeStock(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterWarehouse,
            string filterMaterial,
            string filterLot)
        {
            var results = new List<NegativeStockEntry>();

            // Inner WHERE: exact match for warehouse/material (como no Python), ILIKE para lote
            var conditions = new List<string> { "m.status = 'ATIVO'" };
            if (!string.IsNullOrWhiteSpace(filterWarehouse)) conditions.Add("m.almoxarifado = @wh");
            if (!string.IsNullOrWhiteSpace(filterMaterial))  conditions.Add("m.material = @mat");
            if (!string.IsNullOrWhiteSpace(filterLot))       conditions.Add("m.lote ILIKE @lot");

            var whereInner = string.Join(" AND ", conditions);

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    WITH movs AS (
                        SELECT
                            m.id,
                            m.data_movimento,
                            m.tipo,
                            m.documento_tipo,
                            m.documento_numero,
                            m.material,
                            m.lote,
                            m.almoxarifado,
                            m.quantidade,
                            m.fornecedor,
                            m.vencimento,
                            SUM(
                                CASE
                                    WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA')
                                        THEN m.quantidade
                                    WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO')
                                        THEN -m.quantidade
                                    ELSE 0
                                END
                            ) OVER (
                                PARTITION BY m.almoxarifado, m.material, m.lote
                                ORDER BY m.data_movimento, m.id
                            ) AS saldo
                        FROM movimentos_estoque m
                        WHERE {whereInner}
                    )
                    SELECT
                        mv.id,
                        mv.almoxarifado,
                        COALESCE(a.nome,   '')  AS almox_nome,
                        mv.material,
                        COALESCE(em.descricao, '') AS material_desc,
                        mv.lote,
                        COALESCE(l.nome,   '')  AS lote_nome,
                        mv.data_movimento,
                        mv.tipo,
                        mv.documento_tipo,
                        mv.documento_numero,
                        mv.quantidade,
                        mv.saldo,
                        COALESCE(f.nome,   '')  AS fornecedor_nome,
                        COALESCE(l.validade::text, mv.vencimento::text, '') AS validade
                    FROM movs mv
                    LEFT JOIN almoxarifados a
                        ON  a.codigo  = mv.almoxarifado
                        AND a.versao  = (SELECT MAX(versao) FROM almoxarifados x WHERE x.codigo = a.codigo)
                        AND a.status  = 'ATIVO'
                    LEFT JOIN embalagens em
                        ON  em.codigo = mv.material
                        AND em.versao = (SELECT MAX(versao) FROM embalagens x WHERE x.codigo = em.codigo)
                        AND em.status = 'ATIVO'
                    LEFT JOIN lotes l
                        ON  l.codigo  = mv.lote
                        AND l.versao  = (SELECT MAX(versao) FROM lotes x WHERE x.codigo = l.codigo)
                        AND l.status  = 'ATIVO'
                    LEFT JOIN fornecedores f
                        ON  f.codigo  = l.fornecedor
                        AND f.versao  = (SELECT MAX(versao) FROM fornecedores x WHERE x.codigo = f.codigo)
                        AND f.status  = 'ATIVO'
                    WHERE mv.saldo < 0
                    ORDER BY mv.almoxarifado, mv.material, mv.lote, mv.data_movimento, mv.id";

                if (!string.IsNullOrWhiteSpace(filterWarehouse)) AddParameter(command, "wh",  filterWarehouse.Trim());
                if (!string.IsNullOrWhiteSpace(filterMaterial))  AddParameter(command, "mat", filterMaterial.Trim());
                if (!string.IsNullOrWhiteSpace(filterLot))       AddParameter(command, "lot", "%" + filterLot.Trim() + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new NegativeStockEntry
                        {
                            MovementId    = ReadLong(reader,    "id"),
                            Warehouse     = ReadString(reader,  "almoxarifado"),
                            WarehouseName = ReadString(reader,  "almox_nome"),
                            Material      = ReadString(reader,  "material"),
                            MaterialName  = ReadString(reader,  "material_desc"),
                            Lot           = ReadString(reader,  "lote"),
                            LotName       = ReadString(reader,  "lote_nome"),
                            MovementDate  = ReadString(reader,  "data_movimento"),
                            MovementType  = ReadString(reader,  "tipo"),
                            DocumentType  = ReadString(reader,  "documento_tipo"),
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            Quantity      = ReadDecimal(reader, "quantidade"),
                            RunningBalance = ReadDecimal(reader, "saldo"),
                            SupplierName  = ReadString(reader,  "fornecedor_nome"),
                            Validity      = ReadString(reader,  "validade"),
                        });
                    }
                }
            }

            return results;
        }

        // ── Alert: lot x material inconsistency (bd_inconsistencias_lote_material) ──

        public IReadOnlyCollection<LotMaterialInconsistencyEntry> DiagnoseLotMaterialInconsistencies(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterWarehouse,
            string filterMaterial,
            string filterLot)
        {
            var results = new List<LotMaterialInconsistencyEntry>();

            // Espelha bd_inconsistencias_lote_material.py:
            // CTE 'saldos' calcula saldo por (almoxarifado, material, lote), aplicando
            // sinais +/- conforme o tipo de movimento; HAVING SUM > 0 mantem só quem
            // ainda tem saldo positivo. Em seguida faz JOIN com o cadastro do lote
            // e filtra l.material <> s.material para apontar a divergencia.
            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(filterWarehouse)) conditions.Add("s.almoxarifado = @wh");
            if (!string.IsNullOrWhiteSpace(filterMaterial))  conditions.Add("s.material = @mat");
            if (!string.IsNullOrWhiteSpace(filterLot))       conditions.Add("s.lote ILIKE @lot");

            var whereExtra = conditions.Count > 0
                ? " AND " + string.Join(" AND ", conditions)
                : string.Empty;

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    WITH saldos AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            SUM(CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA')
                                    THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO')
                                    THEN -m.quantidade
                                ELSE 0
                            END) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                        GROUP BY m.almoxarifado, m.material, m.lote
                        HAVING SUM(CASE
                            WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA')
                                THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO')
                                THEN -m.quantidade
                            ELSE 0
                        END) > 0
                    )
                    SELECT
                        s.lote,
                        COALESCE(l.nome, '')          AS lote_nome,
                        s.material                    AS material_mov,
                        COALESCE(em.descricao, '')    AS material_mov_desc,
                        l.material                    AS material_cad,
                        COALESCE(ec.descricao, '')    AS material_cad_desc,
                        s.almoxarifado,
                        COALESCE(a.nome, '')          AS almox_nome,
                        COALESCE(l.validade::text,'') AS validade,
                        COALESCE(f.nome, '')          AS fornecedor_nome,
                        s.saldo
                    FROM saldos s
                    LEFT JOIN lotes l
                        ON  l.codigo  = s.lote
                        AND l.versao  = (SELECT MAX(versao) FROM lotes x WHERE x.codigo = l.codigo)
                        AND l.status  = 'ATIVO'
                    LEFT JOIN embalagens em
                        ON  em.codigo = s.material
                        AND em.versao = (SELECT MAX(versao) FROM embalagens x WHERE x.codigo = em.codigo)
                        AND em.status = 'ATIVO'
                    LEFT JOIN embalagens ec
                        ON  ec.codigo = l.material
                        AND ec.versao = (SELECT MAX(versao) FROM embalagens x WHERE x.codigo = ec.codigo)
                        AND ec.status = 'ATIVO'
                    LEFT JOIN almoxarifados a
                        ON  a.codigo  = s.almoxarifado
                        AND a.versao  = (SELECT MAX(versao) FROM almoxarifados x WHERE x.codigo = a.codigo)
                        AND a.status  = 'ATIVO'
                    LEFT JOIN fornecedores f
                        ON  f.codigo  = l.fornecedor
                        AND f.versao  = (SELECT MAX(versao) FROM fornecedores x WHERE x.codigo = f.codigo)
                        AND f.status  = 'ATIVO'
                    WHERE l.material IS NOT NULL
                      AND l.material <> ''
                      AND s.material <> l.material
                    {whereExtra}
                    ORDER BY s.lote, s.material, s.almoxarifado";

                if (!string.IsNullOrWhiteSpace(filterWarehouse)) AddParameter(command, "wh",  filterWarehouse.Trim());
                if (!string.IsNullOrWhiteSpace(filterMaterial))  AddParameter(command, "mat", filterMaterial.Trim());
                if (!string.IsNullOrWhiteSpace(filterLot))       AddParameter(command, "lot", "%" + filterLot.Trim() + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new LotMaterialInconsistencyEntry
                        {
                            Lot                    = ReadString(reader,  "lote"),
                            LotName                = ReadString(reader,  "lote_nome"),
                            MovementMaterial       = ReadString(reader,  "material_mov"),
                            MovementMaterialName   = ReadString(reader,  "material_mov_desc"),
                            RegisteredMaterial     = ReadString(reader,  "material_cad"),
                            RegisteredMaterialName = ReadString(reader,  "material_cad_desc"),
                            Warehouse              = ReadString(reader,  "almoxarifado"),
                            WarehouseName          = ReadString(reader,  "almox_nome"),
                            Validity               = ReadString(reader,  "validade"),
                            SupplierName           = ReadString(reader,  "fornecedor_nome"),
                            Balance                = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return results;
        }

        // ── Alert: duplicate lots by material ──────────────────────────────────

        public IReadOnlyCollection<DuplicateLotEntry> DiagnoseDuplicateLotsByMaterial(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterMaterial,
            string filterLotName,
            string filterLotCode)
        {
            var results = new List<DuplicateLotEntry>();

            // Build outer WHERE filters (applied after the CTE join, matching Python logic)
            var outerConditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(filterMaterial)) outerConditions.Add("la.material = @mat");
            if (!string.IsNullOrWhiteSpace(filterLotName))  outerConditions.Add("la.nome ILIKE @lotName");
            if (!string.IsNullOrWhiteSpace(filterLotCode))  outerConditions.Add("la.codigo ILIKE @lotCode");

            var extraFilter = outerConditions.Count > 0
                ? "AND " + string.Join(" AND ", outerConditions)
                : string.Empty;

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    WITH lotes_ativos AS (
                        SELECT l.codigo, l.nome, l.material, l.fornecedor, l.validade
                        FROM lotes l
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS versao
                            FROM lotes
                            GROUP BY codigo
                        ) lv ON lv.codigo = l.codigo AND lv.versao = l.versao
                        WHERE l.status = 'ATIVO'
                          AND COALESCE(l.material, '') <> ''
                          AND TRIM(COALESCE(l.nome, '')) <> ''
                    ),
                    grupos_duplicados AS (
                        SELECT
                            material,
                            UPPER(TRIM(COALESCE(nome, ''))) AS nome_norm,
                            COUNT(*) AS qtd_duplicados,
                            STRING_AGG(codigo, ', ' ORDER BY codigo) AS grupo_codigos
                        FROM lotes_ativos
                        GROUP BY material, UPPER(TRIM(COALESCE(nome, '')))
                        HAVING COUNT(*) > 1
                    )
                    SELECT
                        la.material,
                        COALESCE(e.descricao, '') AS material_desc,
                        la.nome,
                        la.codigo,
                        la.fornecedor,
                        COALESCE(f.nome, '') AS fornecedor_nome,
                        la.validade,
                        gd.qtd_duplicados,
                        gd.grupo_codigos
                    FROM lotes_ativos la
                    INNER JOIN grupos_duplicados gd
                        ON gd.material = la.material
                       AND gd.nome_norm = UPPER(TRIM(COALESCE(la.nome, '')))
                    LEFT JOIN embalagens e
                        ON e.codigo = la.material
                       AND e.versao = (SELECT MAX(versao) FROM embalagens x WHERE x.codigo = e.codigo)
                       AND e.status = 'ATIVO'
                    LEFT JOIN fornecedores f
                        ON f.codigo = la.fornecedor
                       AND f.versao = (SELECT MAX(versao) FROM fornecedores x WHERE x.codigo = f.codigo)
                       AND f.status = 'ATIVO'
                    WHERE 1 = 1 {extraFilter}
                    ORDER BY la.material, UPPER(TRIM(la.nome)), la.codigo";

                if (!string.IsNullOrWhiteSpace(filterMaterial)) AddParameter(command, "mat", filterMaterial.Trim());
                if (!string.IsNullOrWhiteSpace(filterLotName))  AddParameter(command, "lotName", "%" + filterLotName.Trim() + "%");
                if (!string.IsNullOrWhiteSpace(filterLotCode))  AddParameter(command, "lotCode", "%" + filterLotCode.Trim() + "%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DuplicateLotEntry
                        {
                            Material     = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_desc"),
                            LotCode      = ReadString(reader, "codigo"),
                            LotName      = ReadString(reader, "nome"),
                            Supplier     = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            Validity     = ReadString(reader, "validade"),
                            DuplicateCount = ReadInt(reader, "qtd_duplicados"),
                            GroupCodes   = ReadString(reader, "grupo_codigos"),
                        });
                    }
                }
            }

            return results;
        }

        // ── Alert: duplicate note movements ───────────────────────────────────
        // Porte fiel de database.py::diagnosticar_movimentos_duplicados_notas
        // - 3 queries: notas_ativas, movimentos ATIVOS de NOTA, notas_itens ATIVOS
        // - Regra 1: movimento cuja (material,lote,almox,qty_chave) NAO esta
        //            nas assinaturas dos itens ATIVOS da versao ativa da nota
        //            E usuario_mov != usuario_nota  => propor inativar
        // - Regra 2: duplicidade exata de assinatura entre movimentos que casam
        //            com os itens ativos (match_rows): mantem menor ID, propoe
        //            inativar os demais
        // - Consolida por NF (UM grupo por NF, uniao das duas regras)

        public IReadOnlyCollection<DuplicateNoteMovementGroup> DiagnoseDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var notas      = new List<RawActiveNote>();
            var movimentos = new List<RawNoteMovement>();
            var itens      = new List<RawNoteItemSignature>();

            using (var connection = _connectionFactory.Open(profile, settings))
            {
                // Query 1: notas ATIVAS (versao max por numero+fornecedor)
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        WITH notas_ativas AS (
                            SELECT n.numero, n.fornecedor, n.versao, n.usuario, n.dt_movimento
                            FROM notas n
                            WHERE n.status = 'ATIVO'
                              AND n.versao = (
                                    SELECT MAX(nx.versao) FROM notas nx
                                    WHERE nx.numero = n.numero AND nx.fornecedor = n.fornecedor
                              )
                        )
                        SELECT numero, fornecedor, versao, usuario, dt_movimento
                        FROM notas_ativas
                        ORDER BY numero, fornecedor";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            notas.Add(new RawActiveNote
                            {
                                Number     = ReadString(rdr, "numero"),
                                Supplier   = ReadString(rdr, "fornecedor"),
                                Version    = ReadString(rdr, "versao"),
                                User       = ReadString(rdr, "usuario"),
                                NoteDate   = ReadString(rdr, "dt_movimento"),
                            });
                        }
                    }
                }

                if (notas.Count == 0) return new List<DuplicateNoteMovementGroup>();

                // Query 2: todos os movimentos ATIVOS de NOTA
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            m.id, m.documento_numero AS numero, m.fornecedor,
                            m.data_movimento, m.almoxarifado, m.material, m.lote,
                            m.quantidade, m.usuario, m.dt_hr_criacao
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.documento_tipo = 'NOTA'
                        ORDER BY m.documento_numero, m.fornecedor, m.id";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            movimentos.Add(new RawNoteMovement
                            {
                                Id           = ReadLong(rdr,    "id"),
                                NoteNumber   = ReadString(rdr,  "numero"),
                                Supplier     = ReadString(rdr,  "fornecedor"),
                                MovementDate = ReadString(rdr,  "data_movimento"),
                                Warehouse    = ReadString(rdr,  "almoxarifado"),
                                Material     = ReadString(rdr,  "material"),
                                Lot          = ReadString(rdr,  "lote"),
                                Quantity     = ReadDecimal(rdr, "quantidade"),
                                User         = ReadString(rdr,  "usuario"),
                                CreatedAt    = ReadString(rdr,  "dt_hr_criacao"),
                            });
                        }
                    }
                }

                // Query 3: assinaturas dos itens ATIVOS das notas ATIVAS
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        WITH notas_ativas AS (
                            SELECT n.numero, n.fornecedor, n.versao
                            FROM notas n
                            WHERE n.status = 'ATIVO'
                              AND n.versao = (
                                    SELECT MAX(nx.versao) FROM notas nx
                                    WHERE nx.numero = n.numero AND nx.fornecedor = n.fornecedor
                              )
                        )
                        SELECT i.numero, i.fornecedor, i.material, i.lote, i.almoxarifado, i.quantidade
                        FROM notas_itens i
                        JOIN notas_ativas n
                          ON  n.numero     = i.numero
                          AND n.fornecedor = i.fornecedor
                          AND n.versao     = i.versao
                        WHERE i.status = 'ATIVO'";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            itens.Add(new RawNoteItemSignature
                            {
                                Number    = ReadString(rdr,  "numero"),
                                Supplier  = ReadString(rdr,  "fornecedor"),
                                Material  = ReadString(rdr,  "material"),
                                Lot       = ReadString(rdr,  "lote"),
                                Warehouse = ReadString(rdr,  "almoxarifado"),
                                Quantity  = ReadDecimal(rdr, "quantidade"),
                            });
                        }
                    }
                }
            }

            return BuildDuplicateNoteGroups(notas, movimentos, itens);
        }

        private static IReadOnlyCollection<DuplicateNoteMovementGroup> BuildDuplicateNoteGroups(
            List<RawActiveNote> notas,
            List<RawNoteMovement> movimentos,
            List<RawNoteItemSignature> itens)
        {
            // Index notas ativas por (numero, fornecedor)
            var notaMap = new Dictionary<string, RawActiveNote>(StringComparer.Ordinal);
            foreach (var n in notas)
            {
                notaMap[NoteKey(n.Number, n.Supplier)] = n;
            }

            // Index movimentos por (numero, fornecedor) — somente os que estao em notas ativas
            var movimentosMap = new Dictionary<string, List<RawNoteMovement>>(StringComparer.Ordinal);
            foreach (var m in movimentos)
            {
                var key = NoteKey(m.NoteNumber, m.Supplier);
                if (!notaMap.ContainsKey(key)) continue;
                if (!movimentosMap.TryGetValue(key, out var bucket))
                {
                    bucket = new List<RawNoteMovement>();
                    movimentosMap[key] = bucket;
                }
                bucket.Add(m);
            }

            // Index assinaturas dos itens por (numero, fornecedor)
            var itensPorNota = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (var i in itens)
            {
                var key = NoteKey(i.Number, i.Supplier);
                if (!itensPorNota.TryGetValue(key, out var set))
                {
                    set = new HashSet<string>(StringComparer.Ordinal);
                    itensPorNota[key] = set;
                }
                set.Add(Signature(i.Material, i.Lot, i.Warehouse, i.Quantity));
            }

            var relatorio = new List<DuplicateNoteMovementGroup>();

            // Ordenacao por numero+fornecedor (igual ao "sorted(movimentos_map.items())" do Python)
            var chavesOrdenadas = movimentosMap.Keys
                .OrderBy(k => k, StringComparer.Ordinal)
                .ToList();

            foreach (var chave in chavesOrdenadas)
            {
                var nota = notaMap[chave];
                var rows = movimentosMap[chave];

                var usuarioNotaNorm = (nota.User ?? string.Empty).Trim().ToLowerInvariant();
                var assinaturasAtivas = itensPorNota.TryGetValue(chave, out var sig)
                    ? sig
                    : new HashSet<string>(StringComparer.Ordinal);

                var matchRows    = new List<RawNoteMovement>();
                var nonmatchRows = new List<RawNoteMovement>();
                foreach (var m in rows)
                {
                    var s = Signature(m.Material, m.Lot, m.Warehouse, m.Quantity);
                    if (assinaturasAtivas.Contains(s)) matchRows.Add(m);
                    else                                nonmatchRows.Add(m);
                }

                var propostas = new Dictionary<long, DuplicateNoteMovementDetail>();

                // Regra 1: item nao pertence a versao ativa + usuario divergente
                foreach (var m in nonmatchRows)
                {
                    var usuarioMovNorm = (m.User ?? string.Empty).Trim().ToLowerInvariant();
                    if (string.Equals(usuarioMovNorm, usuarioNotaNorm, StringComparison.Ordinal)) continue;
                    if (m.Id <= 0) continue;
                    if (propostas.ContainsKey(m.Id)) continue;

                    propostas[m.Id] = BuildDetail(
                        m, nota,
                        reasonCode:  "item_nao_pertence_versao_ativa_e_usuario_diferente",
                        reasonLabel: "Item nao pertence a versao ativa + usuario divergente",
                        keepReference: null);
                }

                // Regra 2: duplicidade exata de assinatura dentro de match_rows (mantem menor ID)
                var porAssinatura = new Dictionary<string, List<RawNoteMovement>>(StringComparer.Ordinal);
                foreach (var m in matchRows)
                {
                    var s = Signature(m.Material, m.Lot, m.Warehouse, m.Quantity);
                    if (!porAssinatura.TryGetValue(s, out var bucket))
                    {
                        bucket = new List<RawNoteMovement>();
                        porAssinatura[s] = bucket;
                    }
                    bucket.Add(m);
                }

                foreach (var dupRows in porAssinatura.Values)
                {
                    if (dupRows.Count <= 1) continue;
                    var ordenados = dupRows.OrderBy(x => x.Id).ToList();
                    var idManter  = ordenados[0].Id;
                    for (int i = 1; i < ordenados.Count; i++)
                    {
                        var m = ordenados[i];
                        if (m.Id <= 0) continue;
                        if (propostas.ContainsKey(m.Id)) continue;

                        propostas[m.Id] = BuildDetail(
                            m, nota,
                            reasonCode:  "duplicidade_exata_mesma_assinatura_item",
                            reasonLabel: "Duplicidade exata da mesma assinatura do item",
                            keepReference: idManter);
                    }
                }

                if (propostas.Count == 0) continue;

                var idsPropostos = propostas.Keys.OrderBy(x => x).ToList();
                var details      = idsPropostos.Select(id => propostas[id]).ToList();

                // "usuarios_mov_ativos": distinto dos usuarios de todos os movimentos ATIVOS da NF
                var usuariosMov = rows
                    .Select(m => (m.User ?? string.Empty).Trim().ToLowerInvariant())
                    .Where(u => u.Length > 0)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(u => u, StringComparer.Ordinal)
                    .ToList();

                // Rotulo da(s) regra(s) para o campo Group.Reason
                var motivosDistintos = details
                    .Select(d => d.ReasonLabel ?? string.Empty)
                    .Where(r => r.Length > 0)
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                relatorio.Add(new DuplicateNoteMovementGroup
                {
                    NoteNumber          = nota.Number,
                    Supplier            = nota.Supplier,
                    NoteVersion         = nota.Version,
                    NoteUser            = nota.User,
                    NoteDate            = nota.NoteDate,
                    TotalActiveMovements = rows.Count,
                    ActiveMovementUsers = string.Join(", ", usuariosMov),
                    Reason              = string.Join(" + ", motivosDistintos),
                    DuplicateMovementIds = idsPropostos,
                    Details             = details,
                });
            }

            return relatorio;
        }

        private static DuplicateNoteMovementDetail BuildDetail(
            RawNoteMovement m,
            RawActiveNote nota,
            string reasonCode,
            string reasonLabel,
            long? keepReference)
        {
            return new DuplicateNoteMovementDetail
            {
                MovementId    = m.Id,
                NoteNumber    = m.NoteNumber,
                Supplier      = m.Supplier,
                Material      = m.Material,
                MaterialName  = string.Empty, // view nao mostra nome do material (apenas codigo)
                Lot           = m.Lot,
                Warehouse     = m.Warehouse,
                Quantity      = m.Quantity,
                MovementDate  = m.MovementDate,
                CreatedAt     = m.CreatedAt,
                MovementUser  = m.User,
                NoteUser      = nota.User,
                ReasonCode    = reasonCode,
                ReasonLabel   = reasonLabel,
                Reason        = reasonLabel,
                KeepReferenceId = keepReference,
            };
        }

        private static string NoteKey(string number, string supplier)
        {
            return (number ?? string.Empty) + "||" + (supplier ?? string.Empty);
        }

        /// <summary>Assinatura (material|lote|almox|qty-normalizada) igual ao Python.</summary>
        private static string Signature(string material, string lot, string warehouse, decimal quantity)
        {
            return (material ?? string.Empty) + "|"
                 + (lot      ?? string.Empty) + "|"
                 + (warehouse ?? string.Empty) + "|"
                 + NormalizeQuantityKey(quantity);
        }

        /// <summary>Equivalente a _quantidade_chave do Python: Decimal.normalize().</summary>
        private static string NormalizeQuantityKey(decimal value)
        {
            var text = value.ToString("0.############################", CultureInfo.InvariantCulture);
            if (text.IndexOf('.') >= 0)
            {
                text = text.TrimEnd('0').TrimEnd('.');
            }
            return text.Length == 0 ? "0" : text;
        }

        public IReadOnlyCollection<DuplicateNoteMovementDetail> LoadDuplicateNoteMovementDetails(DatabaseProfile profile, ConnectionResilienceSettings settings, string noteNumber, string supplier)
        {
            // Mantido por compatibilidade com a API publica. Retorna os movimentos
            // ATIVOS da NF para referencia visual (a tela nova consome Details
            // direto do Group retornado por DiagnoseDuplicateNoteMovements).
            var results = new List<DuplicateNoteMovementDetail>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT me.id, me.documento_numero, me.fornecedor, me.material,
                           e.descricao AS material_nome, me.lote, me.almoxarifado,
                           me.quantidade, me.data_movimento, me.usuario, me.dt_hr_criacao
                    FROM movimentos_estoque me
                    LEFT JOIN embalagens e ON me.material = e.codigo
                    WHERE me.documento_tipo = 'NOTA'
                      AND me.status = 'ATIVO'
                      AND LOWER(me.documento_numero) = @num
                      AND LOWER(me.fornecedor) = @sup
                    ORDER BY me.material, me.lote, me.id";

                AddParameter(command, "num", (noteNumber ?? string.Empty).Trim().ToLowerInvariant());
                AddParameter(command, "sup", (supplier   ?? string.Empty).Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DuplicateNoteMovementDetail
                        {
                            MovementId   = ReadLong(reader,    "id"),
                            NoteNumber   = ReadString(reader,  "documento_numero"),
                            Supplier     = ReadString(reader,  "fornecedor"),
                            Material     = ReadString(reader,  "material"),
                            MaterialName = ReadString(reader,  "material_nome"),
                            Lot          = ReadString(reader,  "lote"),
                            Warehouse    = ReadString(reader,  "almoxarifado"),
                            Quantity     = ReadDecimal(reader, "quantidade"),
                            MovementDate = ReadString(reader,  "data_movimento"),
                            MovementUser = ReadString(reader,  "usuario"),
                            CreatedAt    = ReadString(reader,  "dt_hr_criacao"),
                        });
                    }
                }
            }

            return results;
        }

        public InactivateDuplicatesResult InactivateDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings, long[] movementIds)
        {
            // Espelha database.py::inativar_movimentos_duplicados_notas:
            // - normaliza (>0, distinct, sorted)
            // - SELECT id, status por ANY(@ids) AND documento_tipo='NOTA'
            // - UPDATE status='INATIVO' onde status='ATIVO'
            // - retorna ids_solicitados/encontrados/inativados
            var result = new InactivateDuplicatesResult();

            var cleaned = new List<long>();
            if (movementIds != null)
            {
                foreach (var v in movementIds)
                {
                    if (v > 0) cleaned.Add(v);
                }
            }
            cleaned = cleaned.Distinct().OrderBy(x => x).ToList();
            result.RequestedIds = cleaned;

            if (cleaned.Count == 0) return result;

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                // 1. SELECT para ids_encontrados / ids_ativos
                var paramNames = new List<string>();
                var foundIds   = new List<long>();
                var activeIds  = new List<long>();

                using (var select = connection.CreateCommand())
                {
                    select.Transaction = transaction;
                    for (int i = 0; i < cleaned.Count; i++)
                    {
                        var pName = "id" + i.ToString(CultureInfo.InvariantCulture);
                        paramNames.Add("@" + pName);
                        AddParameter(select, pName, cleaned[i]);
                    }

                    select.CommandText = $@"
                        SELECT m.id, m.status
                        FROM movimentos_estoque m
                        WHERE m.id IN ({string.Join(",", paramNames)})
                          AND m.documento_tipo = 'NOTA'
                        ORDER BY m.id";

                    using (var rdr = select.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var id = ReadLong(rdr, "id");
                            foundIds.Add(id);
                            var status = (ReadString(rdr, "status") ?? string.Empty).Trim().ToUpperInvariant();
                            if (status == "ATIVO") activeIds.Add(id);
                        }
                    }
                }

                result.FoundIds       = foundIds;
                result.InactivatedIds = activeIds;

                if (activeIds.Count > 0)
                {
                    using (var update = connection.CreateCommand())
                    {
                        update.Transaction = transaction;
                        var updParamNames = new List<string>();
                        for (int i = 0; i < activeIds.Count; i++)
                        {
                            var pName = "aid" + i.ToString(CultureInfo.InvariantCulture);
                            updParamNames.Add("@" + pName);
                            AddParameter(update, pName, activeIds[i]);
                        }
                        AddParameter(update, "now", NowText());

                        update.CommandText = $@"
                            UPDATE movimentos_estoque
                            SET status = 'INATIVO', dt_hr_alteracao = @now
                            WHERE id IN ({string.Join(",", updParamNames)})
                              AND documento_tipo = 'NOTA'
                              AND status = 'ATIVO'";

                        update.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            return result;
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private sealed class RawNoteMovement
        {
            public long    Id           { get; set; }
            public string  NoteNumber   { get; set; }
            public string  Supplier     { get; set; }
            public string  Warehouse    { get; set; }
            public string  Material     { get; set; }
            public string  Lot          { get; set; }
            public decimal Quantity     { get; set; }
            public string  MovementDate { get; set; }
            public string  User         { get; set; }
            public string  CreatedAt    { get; set; }
        }

        private sealed class RawActiveNote
        {
            public string Number   { get; set; }
            public string Supplier { get; set; }
            public string Version  { get; set; }
            public string User     { get; set; }
            public string NoteDate { get; set; }
        }

        private sealed class RawNoteItemSignature
        {
            public string  Number    { get; set; }
            public string  Supplier  { get; set; }
            public string  Material  { get; set; }
            public string  Lot       { get; set; }
            public string  Warehouse { get; set; }
            public decimal Quantity  { get; set; }
        }

        private sealed class InboundReceiptReactivationState
        {
            public string Number { get; set; }

            public string Supplier { get; set; }

            public int Version { get; set; }

            public string Status { get; set; }
        }

        private static InboundReceiptReactivationState LoadInboundReceiptForReactivation(DbConnection connection, DbTransaction transaction, string number, string supplier, int version)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT numero, fornecedor, versao, status
                    FROM notas
                    WHERE numero = @numero
                      AND fornecedor = @fornecedor
                      AND versao = @versao
                      AND status = 'CANCELADA'";

                AddParameter(command, "numero", number);
                AddParameter(command, "fornecedor", supplier);
                AddParameter(command, "versao", version);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new InboundReceiptReactivationState
                    {
                        Number = ReadString(reader, "numero"),
                        Supplier = ReadString(reader, "fornecedor"),
                        Version = ReadInt(reader, "versao"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql, params (string name, object value)[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                foreach (var (name, value) in parameters)
                    AddParameter(command, name, value);
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameter(DbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = "@" + name;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }

        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal)?.ToString() ?? string.Empty;
        }

        private static int ReadInt(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal)) return 0;
            return Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static long ReadLong(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal)) return 0L;
            return Convert.ToInt64(reader.GetValue(ordinal));
        }

        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal)) return 0m;
            return Convert.ToDecimal(reader.GetValue(ordinal));
        }

        private static bool ReadBool(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal)) return false;
            var val = reader.GetValue(ordinal);
            if (val is bool b) return b;
            var s = val?.ToString() ?? string.Empty;
            return string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1" || string.Equals(s, "t", StringComparison.OrdinalIgnoreCase);
        }

        private static string NowText() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
