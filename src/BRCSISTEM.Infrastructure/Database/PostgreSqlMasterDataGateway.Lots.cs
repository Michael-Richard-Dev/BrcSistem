using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlMasterDataGateway
    {
        public string GenerateNextLotCode(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE lotes IN EXCLUSIVE MODE");
                    var code = GenerateNextLotCodeLocked(connection, transaction);
                    transaction.Commit();
                    return code;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<LotSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var hasMovements = TableExists(connection, null, "movimentos_estoque");
                command.CommandText = hasMovements
                    ? @"
                        SELECT l.codigo,
                               COALESCE(l.nome, '') AS nome,
                               COALESCE(l.material, '') AS material,
                               COALESCE(l.fornecedor, '') AS fornecedor,
                               COALESCE(l.validade, '') AS validade,
                               l.status,
                               l.versao,
                               COALESCE(f.nome, '') AS fornecedor_nome,
                               COALESCE(e.descricao, '') AS material_desc,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo_total
                        FROM lotes l
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM lotes
                            GROUP BY codigo
                        ) lx ON l.codigo = lx.codigo AND l.versao = lx.max_versao
                        LEFT JOIN (
                            SELECT fo.codigo, fo.nome, fo.versao
                            FROM fornecedores fo
                            INNER JOIN (
                                SELECT codigo, MAX(versao) AS max_versao
                                FROM fornecedores
                                GROUP BY codigo
                            ) fx ON fo.codigo = fx.codigo AND fo.versao = fx.max_versao
                        ) f ON f.codigo = l.fornecedor
                        LEFT JOIN (
                            SELECT e1.codigo, e1.descricao, e1.versao
                            FROM embalagens e1
                            INNER JOIN (
                                SELECT codigo, MAX(versao) AS max_versao
                                FROM embalagens
                                GROUP BY codigo
                            ) ex ON e1.codigo = ex.codigo AND e1.versao = ex.max_versao
                        ) e ON e.codigo = l.material
                        LEFT JOIN movimentos_estoque m ON l.codigo = m.lote AND m.status = 'ATIVO'
                        GROUP BY l.codigo, l.nome, l.material, l.fornecedor, l.validade, l.status, l.versao, f.nome, e.descricao
                        ORDER BY l.codigo"
                    : @"
                        SELECT l.codigo,
                               COALESCE(l.nome, '') AS nome,
                               COALESCE(l.material, '') AS material,
                               COALESCE(l.fornecedor, '') AS fornecedor,
                               COALESCE(l.validade, '') AS validade,
                               l.status,
                               l.versao,
                               COALESCE(f.nome, '') AS fornecedor_nome,
                               COALESCE(e.descricao, '') AS material_desc,
                               0 AS saldo_total
                        FROM lotes l
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM lotes
                            GROUP BY codigo
                        ) lx ON l.codigo = lx.codigo AND l.versao = lx.max_versao
                        LEFT JOIN (
                            SELECT fo.codigo, fo.nome, fo.versao
                            FROM fornecedores fo
                            INNER JOIN (
                                SELECT codigo, MAX(versao) AS max_versao
                                FROM fornecedores
                                GROUP BY codigo
                            ) fx ON fo.codigo = fx.codigo AND fo.versao = fx.max_versao
                        ) f ON f.codigo = l.fornecedor
                        LEFT JOIN (
                            SELECT e1.codigo, e1.descricao, e1.versao
                            FROM embalagens e1
                            INNER JOIN (
                                SELECT codigo, MAX(versao) AS max_versao
                                FROM embalagens
                                GROUP BY codigo
                            ) ex ON e1.codigo = ex.codigo AND e1.versao = ex.max_versao
                        ) e ON e.codigo = l.material
                        ORDER BY l.codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LotSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            ExpirationDate = NormalizeLotValidityText(ReadString(reader, "validade")),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            StockBalance = ReadDecimal(reader, "saldo_total"),
                        });
                    }
                }
            }

            return items;
        }

        public string CreateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveLotRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE lotes IN EXCLUSIVE MODE");

                    var assignedCode = string.IsNullOrWhiteSpace(request.Code)
                        ? GenerateNextLotCodeLocked(connection, transaction)
                        : request.Code.Trim().ToUpperInvariant();

                    if (CodeExists(connection, transaction, "lotes", "codigo", assignedCode))
                    {
                        assignedCode = GenerateNextLotCodeLocked(connection, transaction);
                    }

                    var duplicates = FindActiveLotDuplicatesByNameAndMaterial(connection, transaction, request.Name, request.MaterialCode, null);
                    if (duplicates.Count > 0)
                    {
                        throw new InvalidOperationException(BuildLotDuplicateSameMaterialMessage(request.Name, request.MaterialCode, duplicates));
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "lotes", "codigo", assignedCode);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO lotes (codigo, nome, material, fornecedor, validade, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @material, @fornecedor, @validade, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", assignedCode));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@material", string.IsNullOrWhiteSpace(request.MaterialCode) ? (object)DBNull.Value : request.MaterialCode));
                        command.Parameters.Add(CreateParameter(command, "@fornecedor", request.SupplierCode));
                        command.Parameters.Add(CreateParameter(command, "@validade", request.ExpirationDate));
                        command.Parameters.Add(CreateParameter(command, "@status", request.Status));
                        command.Parameters.Add(CreateParameter(command, "@versao", nextVersion));
                        command.Parameters.Add(CreateParameter(command, "@criacao", now));
                        command.Parameters.Add(CreateParameter(command, "@alteracao", now));
                        command.ExecuteNonQuery();
                    }

                    request.Code = assignedCode;
                    transaction.Commit();
                    return assignedCode;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveLotRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE lotes IN EXCLUSIVE MODE");

                    var current = LoadLatestLot(connection, transaction, request.Code);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Lote nao encontrado.");
                    }

                    var duplicates = FindActiveLotDuplicatesByNameAndMaterial(connection, transaction, request.Name, request.MaterialCode, request.Code);
                    if (duplicates.Count > 0)
                    {
                        throw new InvalidOperationException(BuildLotDuplicateSameMaterialMessage(request.Name, request.MaterialCode, duplicates));
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "lotes", "codigo", request.Code);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO lotes (codigo, nome, material, fornecedor, validade, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @material, @fornecedor, @validade, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@material", string.IsNullOrWhiteSpace(request.MaterialCode) ? (object)DBNull.Value : request.MaterialCode));
                        command.Parameters.Add(CreateParameter(command, "@fornecedor", request.SupplierCode));
                        command.Parameters.Add(CreateParameter(command, "@validade", request.ExpirationDate));
                        command.Parameters.Add(CreateParameter(command, "@status", request.Status));
                        command.Parameters.Add(CreateParameter(command, "@versao", nextVersion));
                        command.Parameters.Add(CreateParameter(command, "@criacao", now));
                        command.Parameters.Add(CreateParameter(command, "@alteracao", now));
                        command.ExecuteNonQuery();
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

        public void InactivateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, string lotCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE lotes IN EXCLUSIVE MODE");

                    var current = LoadLatestLot(connection, transaction, lotCode);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Lote nao encontrado.");
                    }

                    if (string.Equals(current.Status, "INATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Este registro ja esta inativo.");
                    }

                    var stockIssue = BuildLotStockIssueMessage(connection, transaction, current);
                    if (!string.IsNullOrWhiteSpace(stockIssue))
                    {
                        throw new InvalidOperationException(stockIssue);
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "lotes", "codigo", lotCode);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO lotes (codigo, nome, material, fornecedor, validade, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @material, @fornecedor, @validade, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", current.Name));
                        command.Parameters.Add(CreateParameter(command, "@material", string.IsNullOrWhiteSpace(current.MaterialCode) ? (object)DBNull.Value : current.MaterialCode));
                        command.Parameters.Add(CreateParameter(command, "@fornecedor", current.SupplierCode));
                        command.Parameters.Add(CreateParameter(command, "@validade", current.ExpirationDate));
                        command.Parameters.Add(CreateParameter(command, "@status", "INATIVO"));
                        command.Parameters.Add(CreateParameter(command, "@versao", nextVersion));
                        command.Parameters.Add(CreateParameter(command, "@criacao", now));
                        command.Parameters.Add(CreateParameter(command, "@alteracao", now));
                        command.ExecuteNonQuery();
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
    }
}
