using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            int pageSize,
            int offset)
        {
            var results = new List<AuditLogEntry>();
            var where = BuildAuditWhereClause(filterUser, filterAction, filterDateFrom, filterDateTo);

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT id, dt_hr, usuario, acao, detalhes
                    FROM logs_auditoria
                    {where}
                    ORDER BY dt_hr DESC, id DESC
                    LIMIT @pageSize OFFSET @offset";

                AddParameter(command, "pageSize", pageSize);
                AddParameter(command, "offset", offset);
                AddAuditFilterParameters(command, filterUser, filterAction, filterDateFrom, filterDateTo);

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
            string filterDateTo)
        {
            var where = BuildAuditWhereClause(filterUser, filterAction, filterDateFrom, filterDateTo);
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM logs_auditoria {where}";
                AddAuditFilterParameters(command, filterUser, filterAction, filterDateFrom, filterDateTo);
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private static string BuildAuditWhereClause(string filterUser, string filterAction, string filterDateFrom, string filterDateTo)
        {
            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(filterUser)) conditions.Add("LOWER(usuario) LIKE @filterUser");
            if (!string.IsNullOrWhiteSpace(filterAction)) conditions.Add("LOWER(acao) LIKE @filterAction");
            if (!string.IsNullOrWhiteSpace(filterDateFrom)) conditions.Add("dt_hr >= @filterDateFrom::timestamp");
            if (!string.IsNullOrWhiteSpace(filterDateTo)) conditions.Add("dt_hr < (@filterDateTo::date + INTERVAL '1 day')");
            return conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
        }

        private static void AddAuditFilterParameters(DbCommand command, string filterUser, string filterAction, string filterDateFrom, string filterDateTo)
        {
            if (!string.IsNullOrWhiteSpace(filterUser)) AddParameter(command, "filterUser", "%" + filterUser.Trim().ToLowerInvariant() + "%");
            if (!string.IsNullOrWhiteSpace(filterAction)) AddParameter(command, "filterAction", "%" + filterAction.Trim().ToLowerInvariant() + "%");
            if (!string.IsNullOrWhiteSpace(filterDateFrom)) AddParameter(command, "filterDateFrom", filterDateFrom.Trim());
            if (!string.IsNullOrWhiteSpace(filterDateTo)) AddParameter(command, "filterDateTo", filterDateTo.Trim());
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
                    sb.Append($"SELECT '{table}' AS tipo, {numberColumn} AS numero, bloqueado_por, bloqueado_em FROM {table} WHERE bloqueado_por IS NOT NULL");

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
                    SELECT numero, fornecedor, dt_movimento, status, bloqueado_por
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
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
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

        public void RemoveNote(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'NOTA' AND LOWER(fornecedor) = @sup",
                    ("num", number.Trim().ToLowerInvariant()), ("sup", supplier.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM notas_itens WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup",
                    ("num", number.Trim().ToLowerInvariant()), ("sup", supplier.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM notas WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup",
                    ("num", number.Trim().ToLowerInvariant()), ("sup", supplier.Trim().ToLowerInvariant()));

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
                    SELECT numero, dt_movimento, status, bloqueado_por
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
                            Status = ReadString(reader, "status"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
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
                    SELECT ri.material, e.nome AS material_nome, ri.lote, ri.quantidade, ri.unidade, ri.almoxarifado
                    FROM requisicoes_itens ri
                    LEFT JOIN embalagens e ON ri.material = e.codigo
                    WHERE LOWER(ri.numero) = @num
                    ORDER BY ri.material, ri.lote";

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

        public void RemoveRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM movimentos_estoque WHERE LOWER(documento_numero) = @num AND documento_tipo = 'REQUISICAO'",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM requisicoes_itens WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "DELETE FROM requisicoes WHERE LOWER(numero) = @num",
                    ("num", number.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Change note date ───────────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveNotes(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT numero, fornecedor, dt_movimento, status FROM notas WHERE status = 'ATIVO' ORDER BY dt_movimento DESC, numero";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentDateEntry
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return results;
        }

        public void ChangeNoteDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, string newDate)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "UPDATE notas SET dt_movimento = @newDate::date, dt_emissao = @newDate::date WHERE LOWER(numero) = @num AND LOWER(fornecedor) = @sup",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()), ("sup", supplier.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "UPDATE movimentos_estoque SET data_movimento = @newDate::date WHERE documento_tipo = 'NOTA' AND LOWER(documento_numero) = @num AND LOWER(fornecedor) = @sup",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()), ("sup", supplier.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Change transfer date ───────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveTransfers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, dt_movimento, status, almoxarifado_origem, almoxarifado_destino
                    FROM transferencias
                    WHERE status = 'ATIVO'
                    ORDER BY dt_movimento DESC, numero";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DocumentDateEntry
                        {
                            DocumentNumber = ReadString(reader, "numero"),
                            Date = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            OriginWarehouse = ReadString(reader, "almoxarifado_origem"),
                            DestinationWarehouse = ReadString(reader, "almoxarifado_destino"),
                        });
                    }
                }
            }

            return results;
        }

        public void ChangeTransferDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "UPDATE transferencias SET dt_movimento = @newDate::date WHERE LOWER(numero) = @num",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "UPDATE movimentos_estoque SET data_movimento = @newDate::date WHERE documento_tipo = 'TRANSFERENCIA' AND LOWER(documento_numero) = @num",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Change production output date ──────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveProductionOutputs(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DocumentDateEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero, dt_movimento, status, almoxarifado
                    FROM saidas_producao
                    WHERE status = 'ATIVO'
                    ORDER BY dt_movimento DESC, numero";

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
                        });
                    }
                }
            }

            return results;
        }

        public void ChangeProductionOutputDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                ExecuteNonQuery(connection, transaction,
                    "UPDATE saidas_producao SET dt_movimento = @newDate::date WHERE LOWER(numero) = @num",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()));

                ExecuteNonQuery(connection, transaction,
                    "UPDATE movimentos_estoque SET data_movimento = @newDate::date WHERE documento_tipo = 'SAIDA_PRODUCAO' AND LOWER(documento_numero) = @num",
                    ("newDate", newDate.Trim()), ("num", number.Trim().ToLowerInvariant()));

                transaction.Commit();
            }
        }

        // ── Alert: divergent lot entries ───────────────────────────────────────

        public IReadOnlyCollection<DivergentLotEntry> DiagnoseDivergentLotEntries(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var results = new List<DivergentLotEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT me.id, me.documento_numero, me.fornecedor, me.almoxarifado,
                           me.material, e.nome AS material_nome, me.lote,
                           me.quantidade, me.data_movimento, me.status
                    FROM movimentos_estoque me
                    LEFT JOIN embalagens e ON me.material = e.codigo
                    WHERE me.documento_tipo = 'NOTA'
                      AND me.status = 'ATIVO'
                      AND NOT EXISTS (
                          SELECT 1 FROM notas_itens ni
                          WHERE LOWER(ni.numero) = LOWER(me.documento_numero)
                            AND LOWER(ni.fornecedor) = LOWER(me.fornecedor)
                            AND ni.lote = me.lote
                      )
                    ORDER BY me.data_movimento DESC, me.documento_numero";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DivergentLotEntry
                        {
                            MovementId = ReadLong(reader, "id"),
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            Material = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            MovementDate = ReadString(reader, "data_movimento"),
                            Status = ReadString(reader, "status"),
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

        public IReadOnlyCollection<DuplicateNoteMovementGroup> DiagnoseDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            // Load all active note movements and diagnose in memory
            var allMovements = new List<RawNoteMovement>();

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT me.id, me.documento_numero, me.fornecedor, me.almoxarifado,
                           me.material, me.lote, me.quantidade, me.data_movimento, me.versao_nota
                    FROM movimentos_estoque me
                    WHERE me.documento_tipo = 'NOTA' AND me.status = 'ATIVO'
                    ORDER BY me.documento_numero, me.fornecedor, me.material, me.lote, me.id";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allMovements.Add(new RawNoteMovement
                        {
                            Id = ReadLong(reader, "id"),
                            NoteNumber = ReadString(reader, "documento_numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            Material = ReadString(reader, "material"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            Date = ReadString(reader, "data_movimento"),
                            NoteVersion = ReadString(reader, "versao_nota"),
                        });
                    }
                }
            }

            // Diagnose duplicates using the same logic as the Python algorithm
            return FindDuplicateGroups(allMovements);
        }

        private static IReadOnlyCollection<DuplicateNoteMovementGroup> FindDuplicateGroups(List<RawNoteMovement> allMovements)
        {
            var groups = new List<DuplicateNoteMovementGroup>();

            // Group by note + supplier
            var byNote = new Dictionary<string, List<RawNoteMovement>>(StringComparer.OrdinalIgnoreCase);
            foreach (var mov in allMovements)
            {
                var key = $"{mov.NoteNumber}||{mov.Supplier}";
                if (!byNote.ContainsKey(key)) byNote[key] = new List<RawNoteMovement>();
                byNote[key].Add(mov);
            }

            foreach (var pair in byNote)
            {
                var noteMovs = pair.Value;
                if (noteMovs.Count < 2) continue;

                // Rule 1: movements not belonging to the current latest version
                var latestVersion = string.Empty;
                foreach (var m in noteMovs)
                {
                    if (string.Compare(m.NoteVersion, latestVersion, StringComparison.OrdinalIgnoreCase) > 0)
                        latestVersion = m.NoteVersion;
                }

                var staleIds = new List<long>();
                foreach (var m in noteMovs)
                {
                    if (!string.IsNullOrEmpty(latestVersion) &&
                        !string.Equals(m.NoteVersion, latestVersion, StringComparison.OrdinalIgnoreCase))
                    {
                        staleIds.Add(m.Id);
                    }
                }

                if (staleIds.Count > 0)
                {
                    groups.Add(new DuplicateNoteMovementGroup
                    {
                        NoteNumber = noteMovs[0].NoteNumber,
                        Supplier = noteMovs[0].Supplier,
                        Reason = "Movimentos de versao desatualizada da nota",
                        DuplicateMovementIds = staleIds,
                    });
                    continue;
                }

                // Rule 2: exact signature duplicates (material+lot+qty+warehouse)
                var signatures = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
                foreach (var m in noteMovs)
                {
                    var sig = $"{m.Material}||{m.Lot}||{m.Quantity}||{m.Warehouse}";
                    if (!signatures.ContainsKey(sig)) signatures[sig] = new List<long>();
                    signatures[sig].Add(m.Id);
                }

                var duplicateIds = new List<long>();
                foreach (var sigPair in signatures)
                {
                    if (sigPair.Value.Count > 1)
                    {
                        // Keep first, mark rest as duplicates
                        for (int i = 1; i < sigPair.Value.Count; i++)
                            duplicateIds.Add(sigPair.Value[i]);
                    }
                }

                if (duplicateIds.Count > 0)
                {
                    groups.Add(new DuplicateNoteMovementGroup
                    {
                        NoteNumber = noteMovs[0].NoteNumber,
                        Supplier = noteMovs[0].Supplier,
                        Reason = "Movimentos duplicados (mesma assinatura)",
                        DuplicateMovementIds = duplicateIds,
                    });
                }
            }

            return groups;
        }

        public IReadOnlyCollection<DuplicateNoteMovementDetail> LoadDuplicateNoteMovementDetails(DatabaseProfile profile, ConnectionResilienceSettings settings, string noteNumber, string supplier)
        {
            var results = new List<DuplicateNoteMovementDetail>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT me.id, me.documento_numero, me.fornecedor, me.material,
                           e.nome AS material_nome, me.lote, me.almoxarifado,
                           me.quantidade, me.data_movimento
                    FROM movimentos_estoque me
                    LEFT JOIN embalagens e ON me.material = e.codigo
                    WHERE me.documento_tipo = 'NOTA'
                      AND me.status = 'ATIVO'
                      AND LOWER(me.documento_numero) = @num
                      AND LOWER(me.fornecedor) = @sup
                    ORDER BY me.material, me.lote, me.id";

                AddParameter(command, "num", noteNumber.Trim().ToLowerInvariant());
                AddParameter(command, "sup", supplier.Trim().ToLowerInvariant());

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new DuplicateNoteMovementDetail
                        {
                            MovementId = ReadLong(reader, "id"),
                            NoteNumber = ReadString(reader, "documento_numero"),
                            Supplier = ReadString(reader, "fornecedor"),
                            Material = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Lot = ReadString(reader, "lote"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            MovementDate = ReadString(reader, "data_movimento"),
                        });
                    }
                }
            }

            return results;
        }

        public void InactivateDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings, long[] movementIds)
        {
            if (movementIds == null || movementIds.Length == 0) return;

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;

                // Build id list as parameters
                var paramNames = new List<string>();
                for (int i = 0; i < movementIds.Length; i++)
                {
                    var pName = $"id{i}";
                    paramNames.Add("@" + pName);
                    AddParameter(command, pName, movementIds[i]);
                }

                AddParameter(command, "now", NowText());
                command.CommandText = $@"
                    UPDATE movimentos_estoque
                    SET status = 'INATIVO', dt_hr_alteracao = @now
                    WHERE id IN ({string.Join(",", paramNames)})
                      AND documento_tipo = 'NOTA'
                      AND status = 'ATIVO'";

                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private sealed class RawNoteMovement
        {
            public long Id { get; set; }
            public string NoteNumber { get; set; }
            public string Supplier { get; set; }
            public string Warehouse { get; set; }
            public string Material { get; set; }
            public string Lot { get; set; }
            public decimal Quantity { get; set; }
            public string Date { get; set; }
            public string NoteVersion { get; set; }
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
