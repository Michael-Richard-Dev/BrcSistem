using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlMasterDataGateway : IMasterDataGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlMasterDataGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<SupplierSummary> LoadSuppliers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<SupplierSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT f1.codigo,
                           f1.nome,
                           COALESCE(f1.cnpj, '') AS cnpj,
                           COALESCE(f1.cidade, '') AS cidade,
                           COALESCE(f1.habilitado_brc, FALSE) AS habilitado_brc,
                           f1.status,
                           f1.versao
                    FROM fornecedores f1
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM fornecedores
                        GROUP BY codigo
                    ) f2 ON f1.codigo = f2.codigo AND f1.versao = f2.max_versao
                    ORDER BY f1.codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SupplierSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            Cnpj = ReadString(reader, "cnpj"),
                            City = ReadString(reader, "cidade"),
                            IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public void CreateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveSupplierRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE fornecedores IN EXCLUSIVE MODE");

                    if (SupplierCodeExists(connection, transaction, request.Code))
                    {
                        throw new InvalidOperationException("O codigo " + request.Code + " ja existe.");
                    }

                    if (SupplierCnpjExists(connection, transaction, request.Cnpj, request.Code))
                    {
                        throw new InvalidOperationException("O CNPJ informado ja esta cadastrado em outro fornecedor.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "fornecedores", "codigo", request.Code);
                    var now = NowText();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO fornecedores (codigo, nome, cnpj, cidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @cnpj, @cidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@cnpj", request.Cnpj));
                        command.Parameters.Add(CreateParameter(command, "@cidade", request.City));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", request.IsBrcEnabled));
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

        public void UpdateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveSupplierRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE fornecedores IN EXCLUSIVE MODE");

                    if (LoadLatestSupplier(connection, transaction, request.Code) == null)
                    {
                        throw new InvalidOperationException("Fornecedor nao encontrado.");
                    }

                    if (SupplierCnpjExists(connection, transaction, request.Cnpj, request.Code))
                    {
                        throw new InvalidOperationException("O CNPJ informado ja esta cadastrado em outro fornecedor.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "fornecedores", "codigo", request.Code);
                    var now = NowText();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO fornecedores (codigo, nome, cnpj, cidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @cnpj, @cidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@cnpj", request.Cnpj));
                        command.Parameters.Add(CreateParameter(command, "@cidade", request.City));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", request.IsBrcEnabled));
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

        public void InactivateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE fornecedores IN EXCLUSIVE MODE");

                    var current = LoadLatestSupplier(connection, transaction, supplierCode);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Fornecedor nao encontrado.");
                    }

                    var stockIssue = BuildSupplierStockIssueMessage(connection, transaction, supplierCode);
                    if (!string.IsNullOrWhiteSpace(stockIssue))
                    {
                        throw new InvalidOperationException(stockIssue);
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "fornecedores", "codigo", supplierCode);
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO fornecedores (codigo, nome, cnpj, cidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @cnpj, @cidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", current.Name));
                        command.Parameters.Add(CreateParameter(command, "@cnpj", current.Cnpj));
                        command.Parameters.Add(CreateParameter(command, "@cidade", current.City));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", current.IsBrcEnabled));
                        command.Parameters.Add(CreateParameter(command, "@status", "INATIVO"));
                        command.Parameters.Add(CreateParameter(command, "@versao", nextVersion));
                        command.Parameters.Add(CreateParameter(command, "@criacao", current.CreatedAt));
                        command.Parameters.Add(CreateParameter(command, "@alteracao", NowText()));
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

        public IReadOnlyCollection<PackagingSummary> LoadPackagings(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<PackagingSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var hasMovements = TableExists(connection, null, "movimentos_estoque");
                command.CommandText = hasMovements
                    ? @"
                        SELECT e1.codigo,
                               e1.descricao,
                               COALESCE(e1.habilitado_brc, FALSE) AS habilitado_brc,
                               e1.status,
                               e1.versao,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo_total
                        FROM embalagens e1
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM embalagens
                            GROUP BY codigo
                        ) e2 ON e1.codigo = e2.codigo AND e1.versao = e2.max_versao
                        LEFT JOIN movimentos_estoque m ON e1.codigo = m.material AND m.status = 'ATIVO'
                        GROUP BY e1.codigo, e1.descricao, e1.habilitado_brc, e1.status, e1.versao
                        ORDER BY e1.codigo"
                    : @"
                        SELECT e1.codigo,
                               e1.descricao,
                               COALESCE(e1.habilitado_brc, FALSE) AS habilitado_brc,
                               e1.status,
                               e1.versao,
                               0 AS saldo_total
                        FROM embalagens e1
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM embalagens
                            GROUP BY codigo
                        ) e2 ON e1.codigo = e2.codigo AND e1.versao = e2.max_versao
                        ORDER BY e1.codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new PackagingSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Description = ReadString(reader, "descricao"),
                            IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            StockBalance = ReadDecimal(reader, "saldo_total"),
                        });
                    }
                }
            }

            return items;
        }

        public void CreatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, SavePackagingRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE embalagens IN EXCLUSIVE MODE");

                    if (CodeExists(connection, transaction, "embalagens", "codigo", request.Code))
                    {
                        throw new InvalidOperationException("O codigo " + request.Code + " ja existe. Use outro codigo.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "embalagens", "codigo", request.Code);
                    var now = NowText();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO embalagens (codigo, descricao, unidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @unidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", request.Description));
                        command.Parameters.Add(CreateParameter(command, "@unidade", DBNull.Value));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", request.IsBrcEnabled));
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

        public void UpdatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, SavePackagingRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE embalagens IN EXCLUSIVE MODE");

                    var current = LoadLatestPackaging(connection, transaction, request.Code);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Embalagem nao encontrada.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "embalagens", "codigo", request.Code);
                    var now = NowText();

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO embalagens (codigo, descricao, unidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @unidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", request.Description));
                        command.Parameters.Add(CreateParameter(command, "@unidade", current.Unit));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", request.IsBrcEnabled));
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

        public void InactivatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, string packagingCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE embalagens IN EXCLUSIVE MODE");

                    var current = LoadLatestPackaging(connection, transaction, packagingCode);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Embalagem nao encontrada.");
                    }

                    var stockIssue = BuildPackagingStockIssueMessage(connection, transaction, packagingCode, current.Description);
                    if (!string.IsNullOrWhiteSpace(stockIssue))
                    {
                        throw new InvalidOperationException(stockIssue);
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "embalagens", "codigo", packagingCode);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO embalagens (codigo, descricao, unidade, habilitado_brc, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @unidade, @habilitado_brc, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", current.Description));
                        command.Parameters.Add(CreateParameter(command, "@unidade", current.Unit));
                        command.Parameters.Add(CreateParameter(command, "@habilitado_brc", current.IsBrcEnabled));
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

        public IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<WarehouseSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT a1.codigo,
                           a1.nome,
                           COALESCE(a1.empresa, '') AS empresa,
                           COALESCE(a1.empresa_nome, '') AS empresa_nome,
                           a1.status,
                           a1.versao
                    FROM almoxarifados a1
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM almoxarifados
                        GROUP BY codigo
                    ) a2 ON a1.codigo = a2.codigo AND a1.versao = a2.max_versao
                    ORDER BY a1.codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new WarehouseSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            CompanyCode = ReadString(reader, "empresa"),
                            CompanyName = ReadString(reader, "empresa_nome"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public void CreateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveWarehouseRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE almoxarifados IN EXCLUSIVE MODE");

                    if (CodeExists(connection, transaction, "almoxarifados", "codigo", request.Code))
                    {
                        throw new InvalidOperationException("O codigo " + request.Code + " ja existe. Use outro codigo.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "almoxarifados", "codigo", request.Code);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO almoxarifados (codigo, nome, empresa, empresa_nome, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @empresa, @empresa_nome, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@empresa", request.CompanyCode));
                        command.Parameters.Add(CreateParameter(command, "@empresa_nome", request.CompanyName));
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

        public void UpdateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveWarehouseRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE almoxarifados IN EXCLUSIVE MODE");

                    if (LoadLatestWarehouse(connection, transaction, request.Code) == null)
                    {
                        throw new InvalidOperationException("Almoxarifado nao encontrado.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "almoxarifados", "codigo", request.Code);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO almoxarifados (codigo, nome, empresa, empresa_nome, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @empresa, @empresa_nome, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.Name));
                        command.Parameters.Add(CreateParameter(command, "@empresa", request.CompanyCode));
                        command.Parameters.Add(CreateParameter(command, "@empresa_nome", request.CompanyName));
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

        public void InactivateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE almoxarifados IN EXCLUSIVE MODE");

                    var current = LoadLatestWarehouse(connection, transaction, warehouseCode);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Almoxarifado nao encontrado.");
                    }

                    if (string.Equals(current.Status, "INATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Este registro ja esta inativo.");
                    }

                    var stockIssue = BuildWarehouseStockIssueMessage(connection, transaction, warehouseCode);
                    if (!string.IsNullOrWhiteSpace(stockIssue))
                    {
                        throw new InvalidOperationException(stockIssue);
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "almoxarifados", "codigo", warehouseCode);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO almoxarifados (codigo, nome, empresa, empresa_nome, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @nome, @empresa, @empresa_nome, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));
                        command.Parameters.Add(CreateParameter(command, "@nome", current.Name));
                        command.Parameters.Add(CreateParameter(command, "@empresa", current.CompanyCode));
                        command.Parameters.Add(CreateParameter(command, "@empresa_nome", current.CompanyName));
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

        private static SupplierRecord LoadLatestSupplier(DbConnection connection, DbTransaction transaction, string supplierCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT codigo,
                           nome,
                           COALESCE(cnpj, '') AS cnpj,
                           COALESCE(cidade, '') AS cidade,
                           COALESCE(habilitado_brc, FALSE) AS habilitado_brc,
                           status,
                           dt_hr_criacao
                    FROM fornecedores
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", supplierCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new SupplierRecord
                    {
                        Code = ReadString(reader, "codigo"),
                        Name = ReadString(reader, "nome"),
                        Cnpj = ReadString(reader, "cnpj"),
                        City = ReadString(reader, "cidade"),
                        IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                        Status = ReadString(reader, "status"),
                        CreatedAt = ReadString(reader, "dt_hr_criacao"),
                    };
                }
            }
        }

        private static PackagingRecord LoadLatestPackaging(DbConnection connection, DbTransaction transaction, string packagingCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT codigo,
                           descricao,
                           unidade,
                           COALESCE(habilitado_brc, FALSE) AS habilitado_brc,
                           status
                    FROM embalagens
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", packagingCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new PackagingRecord
                    {
                        Code = ReadString(reader, "codigo"),
                        Description = ReadString(reader, "descricao"),
                        Unit = ReadString(reader, "unidade"),
                        IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static WarehouseRecord LoadLatestWarehouse(DbConnection connection, DbTransaction transaction, string warehouseCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT codigo,
                           nome,
                           COALESCE(empresa, '') AS empresa,
                           COALESCE(empresa_nome, '') AS empresa_nome,
                           status
                    FROM almoxarifados
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", warehouseCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new WarehouseRecord
                    {
                        Code = ReadString(reader, "codigo"),
                        Name = ReadString(reader, "nome"),
                        CompanyCode = ReadString(reader, "empresa"),
                        CompanyName = ReadString(reader, "empresa_nome"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static bool SupplierCodeExists(DbConnection connection, DbTransaction transaction, string supplierCode)
        {
            return CodeExists(connection, transaction, "fornecedores", "codigo", supplierCode);
        }

        private static bool SupplierCnpjExists(DbConnection connection, DbTransaction transaction, string cnpj, string supplierCodeToIgnore)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(*)
                    FROM fornecedores
                    WHERE cnpj = @cnpj
                      AND (@codigo IS NULL OR codigo <> @codigo)";
                command.Parameters.Add(CreateParameter(command, "@cnpj", cnpj));
                command.Parameters.Add(CreateParameter(command, "@codigo", string.IsNullOrWhiteSpace(supplierCodeToIgnore) ? (object)DBNull.Value : supplierCodeToIgnore));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
        }

        private static bool CodeExists(DbConnection connection, DbTransaction transaction, string tableName, string codeColumn, string codeValue)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM " + tableName + " WHERE " + codeColumn + " = @codigo";
                command.Parameters.Add(CreateParameter(command, "@codigo", codeValue));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
        }

        private static int GetNextVersion(DbConnection connection, DbTransaction transaction, string tableName, string codeColumn, string codeValue)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COALESCE(MAX(versao), 0) + 1 FROM " + tableName + " WHERE " + codeColumn + " = @codigo";
                command.Parameters.Add(CreateParameter(command, "@codigo", codeValue));
                return Convert.ToInt32(command.ExecuteScalar() ?? 1);
            }
        }

        private static string BuildSupplierStockIssueMessage(DbConnection connection, DbTransaction transaction, string supplierCode)
        {
            if (!TableExists(connection, transaction, "lotes") || !TableExists(connection, transaction, "movimentos_estoque"))
            {
                return null;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT l.codigo AS lote_codigo,
                           COALESCE(l.nome, l.codigo) AS lote_nome,
                           m.material,
                           COALESCE(e.descricao, m.material) AS material_desc,
                           SUM(
                               CASE
                                   WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                   WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                   ELSE 0
                               END
                           ) AS saldo
                    FROM lotes l
                    INNER JOIN movimentos_estoque m ON l.codigo = m.lote AND m.status = 'ATIVO'
                    LEFT JOIN embalagens e ON m.material = e.codigo
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo AND x.status = 'ATIVO'
                        )
                    WHERE l.fornecedor = @fornecedor
                      AND l.versao = (
                          SELECT MAX(versao)
                          FROM lotes x
                          WHERE x.codigo = l.codigo AND x.status = 'ATIVO'
                      )
                    GROUP BY l.codigo, l.nome, m.material, e.descricao
                    HAVING SUM(
                        CASE
                            WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                            ELSE 0
                        END
                    ) > 0
                    ORDER BY l.codigo
                    LIMIT 5";
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));

                using (var reader = command.ExecuteReader())
                {
                    var lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(string.Format(
                            "{0}. Lote: {1} - {2}\n   Material: {3}\n   Saldo: {4:N2}",
                            lines.Count + 1,
                            ReadString(reader, "lote_codigo"),
                            ReadString(reader, "lote_nome"),
                            ReadString(reader, "material_desc"),
                            ReadDecimal(reader, "saldo")));
                    }

                    if (lines.Count == 0)
                    {
                        return null;
                    }

                    return "Nao e possivel inativar o fornecedor '" + supplierCode + "'.\n\n"
                        + "Existem lotes deste fornecedor com saldo em estoque:\n\n"
                        + string.Join("\n\n", lines.ToArray())
                        + "\n\nPara inativar este fornecedor, primeiro e necessario zerar o saldo de todos os lotes vinculados a ele.";
                }
            }
        }

        private static string BuildPackagingStockIssueMessage(DbConnection connection, DbTransaction transaction, string packagingCode, string description)
        {
            if (!TableExists(connection, transaction, "movimentos_estoque"))
            {
                return null;
            }

            var hasLots = TableExists(connection, transaction, "lotes");
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = hasLots
                    ? @"
                        SELECT m.almoxarifado,
                               m.lote,
                               COALESCE(l.nome, m.lote) AS lote_nome,
                               SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ) AS saldo
                        FROM movimentos_estoque m
                        LEFT JOIN lotes l ON m.lote = l.codigo
                            AND l.versao = (
                                SELECT MAX(versao)
                                FROM lotes x
                                WHERE x.codigo = l.codigo AND x.status = 'ATIVO'
                            )
                        WHERE m.material = @codigo
                          AND m.status = 'ATIVO'
                        GROUP BY m.almoxarifado, m.lote, l.nome
                        HAVING SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ) > 0
                        ORDER BY m.almoxarifado, m.lote
                        LIMIT 5"
                    : @"
                        SELECT m.almoxarifado,
                               m.lote,
                               m.lote AS lote_nome,
                               SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.material = @codigo
                          AND m.status = 'ATIVO'
                        GROUP BY m.almoxarifado, m.lote
                        HAVING SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ) > 0
                        ORDER BY m.almoxarifado, m.lote
                        LIMIT 5";
                command.Parameters.Add(CreateParameter(command, "@codigo", packagingCode));

                using (var reader = command.ExecuteReader())
                {
                    var lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(string.Format(
                            "{0}. Almoxarifado: {1}\n   Lote: {2} - {3}\n   Saldo: {4:N2}",
                            lines.Count + 1,
                            ReadString(reader, "almoxarifado"),
                            ReadString(reader, "lote"),
                            ReadString(reader, "lote_nome"),
                            ReadDecimal(reader, "saldo")));
                    }

                    if (lines.Count == 0)
                    {
                        return null;
                    }

                    var title = packagingCode + (string.IsNullOrWhiteSpace(description) ? string.Empty : " - " + description);
                    return "Nao e possivel inativar a embalagem '" + title + "'.\n\n"
                        + "Existem movimentacoes deste material com saldo em estoque:\n\n"
                        + string.Join("\n\n", lines.ToArray())
                        + "\n\nPara inativar esta embalagem, primeiro e necessario zerar o saldo de todos os lotes vinculados a ela.";
                }
            }
        }

        private static string BuildWarehouseStockIssueMessage(DbConnection connection, DbTransaction transaction, string warehouseCode)
        {
            if (!TableExists(connection, transaction, "movimentos_estoque"))
            {
                return null;
            }

            var hasLots = TableExists(connection, transaction, "lotes");
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = hasLots
                    ? @"
                        SELECT m.material,
                               COALESCE(e.descricao, m.material) AS material_desc,
                               m.lote,
                               COALESCE(l.nome, m.lote) AS lote_nome,
                               SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ) AS saldo
                        FROM movimentos_estoque m
                        LEFT JOIN embalagens e ON m.material = e.codigo
                            AND e.versao = (
                                SELECT MAX(versao)
                                FROM embalagens x
                                WHERE x.codigo = e.codigo AND x.status = 'ATIVO'
                            )
                        LEFT JOIN lotes l ON m.lote = l.codigo
                            AND l.versao = (
                                SELECT MAX(versao)
                                FROM lotes x
                                WHERE x.codigo = l.codigo AND x.status = 'ATIVO'
                            )
                        WHERE m.almoxarifado = @almoxarifado
                          AND m.status = 'ATIVO'
                        GROUP BY m.material, e.descricao, m.lote, l.nome
                        HAVING SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ) > 0
                        ORDER BY m.material, m.lote
                        LIMIT 5"
                    : @"
                        SELECT m.material,
                               m.material AS material_desc,
                               m.lote,
                               m.lote AS lote_nome,
                               SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.almoxarifado = @almoxarifado
                          AND m.status = 'ATIVO'
                        GROUP BY m.material, m.lote
                        HAVING SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ) > 0
                        ORDER BY m.material, m.lote
                        LIMIT 5";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));

                using (var reader = command.ExecuteReader())
                {
                    var lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(string.Format(
                            "{0}. Material: {1}\n   Lote: {2} - {3}\n   Saldo: {4:N2}",
                            lines.Count + 1,
                            ReadString(reader, "material_desc"),
                            ReadString(reader, "lote"),
                            ReadString(reader, "lote_nome"),
                            ReadDecimal(reader, "saldo")));
                    }

                    if (lines.Count == 0)
                    {
                        return null;
                    }

                    return "Nao e possivel inativar o almoxarifado '" + warehouseCode + "'.\n\n"
                        + "Existem materiais com saldo em estoque neste almoxarifado:\n\n"
                        + string.Join("\n\n", lines.ToArray())
                        + "\n\nPara inativar este almoxarifado, primeiro e necessario zerar o saldo de todos os materiais nele.";
                }
            }
        }

        private static bool TableExists(DbConnection connection, DbTransaction transaction, string tableName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT EXISTS (
                        SELECT 1
                        FROM information_schema.tables
                        WHERE table_schema = current_schema()
                          AND table_name = @table_name
                    )";
                command.Parameters.Add(CreateParameter(command, "@table_name", tableName));
                return Convert.ToBoolean(command.ExecuteScalar() ?? false);
            }
        }

        private static void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private static DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal));
        }

        private static bool ReadBoolean(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal));
        }

        private static int ReadInt(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0M : Convert.ToDecimal(reader.GetValue(ordinal));
        }

        private static string NowText()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private sealed class SupplierRecord
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public string Cnpj { get; set; }

            public string City { get; set; }

            public bool IsBrcEnabled { get; set; }

            public string Status { get; set; }

            public string CreatedAt { get; set; }
        }

        private sealed class PackagingRecord
        {
            public string Code { get; set; }

            public string Description { get; set; }

            public string Unit { get; set; }

            public bool IsBrcEnabled { get; set; }

            public string Status { get; set; }
        }

        private sealed class WarehouseRecord
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public string CompanyCode { get; set; }

            public string CompanyName { get; set; }

            public string Status { get; set; }
        }
    }
}
