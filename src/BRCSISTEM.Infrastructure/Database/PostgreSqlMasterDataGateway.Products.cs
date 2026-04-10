using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlMasterDataGateway
    {
        public IReadOnlyCollection<ProductSummary> LoadProducts(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<ProductSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT p1.codigo,
                           COALESCE(p1.descricao, '') AS descricao,
                           COALESCE(p1.tipo, '') AS tipo,
                           p1.status,
                           p1.versao
                    FROM produtos p1
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM produtos
                        WHERE codigo ~ '^[0-9]+$'
                        GROUP BY codigo
                    ) p2 ON p1.codigo = p2.codigo AND p1.versao = p2.max_versao
                    ORDER BY CAST(p1.codigo AS INTEGER)";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ProductSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Description = ReadString(reader, "descricao"),
                            Type = ReadString(reader, "tipo"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public void CreateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE produtos IN EXCLUSIVE MODE");

                    if (ActiveProductCodeExists(connection, transaction, request.Code))
                    {
                        throw new InvalidOperationException("Ja existe um produto ativo com esse codigo.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "produtos", "codigo", request.Code);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO produtos (codigo, descricao, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", request.Description));
                        command.Parameters.Add(CreateParameter(command, "@tipo", DBNull.Value));
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

        public void UpdateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE produtos IN EXCLUSIVE MODE");

                    var current = LoadLatestProduct(connection, transaction, request.Code);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Produto nao encontrado.");
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "produtos", "codigo", request.Code);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO produtos (codigo, descricao, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", request.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", request.Description));
                        command.Parameters.Add(CreateParameter(command, "@tipo", string.IsNullOrWhiteSpace(current.Type) ? (object)DBNull.Value : current.Type));
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

        public void InactivateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, string productCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE produtos IN EXCLUSIVE MODE");

                    var current = LoadLatestProduct(connection, transaction, productCode);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Produto nao encontrado.");
                    }

                    if (string.Equals(current.Status, "INATIVO", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Este registro ja esta inativo.");
                    }

                    var usageIssue = BuildProductUsageIssueMessage(connection, transaction, productCode, current.Description, current.Type);
                    if (!string.IsNullOrWhiteSpace(usageIssue))
                    {
                        throw new InvalidOperationException(usageIssue);
                    }

                    var nextVersion = GetNextVersion(connection, transaction, "produtos", "codigo", productCode);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO produtos (codigo, descricao, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@codigo, @descricao, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));
                        command.Parameters.Add(CreateParameter(command, "@descricao", current.Description));
                        command.Parameters.Add(CreateParameter(command, "@tipo", string.IsNullOrWhiteSpace(current.Type) ? (object)DBNull.Value : current.Type));
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

        private static ProductRecord LoadLatestProduct(DbConnection connection, DbTransaction transaction, string productCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT codigo,
                           COALESCE(descricao, '') AS descricao,
                           COALESCE(tipo, '') AS tipo,
                           status
                    FROM produtos
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", productCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new ProductRecord
                    {
                        Code = ReadString(reader, "codigo"),
                        Description = ReadString(reader, "descricao"),
                        Type = ReadString(reader, "tipo"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static bool ActiveProductCodeExists(DbConnection connection, DbTransaction transaction, string productCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(*)
                    FROM produtos
                    WHERE codigo = @codigo
                      AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@codigo", productCode));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
        }

        private static string BuildProductUsageIssueMessage(DbConnection connection, DbTransaction transaction, string productCode, string productDescription, string productType)
        {
            if (!TableExists(connection, transaction, "movimentos_estoque")
                || !TableExists(connection, transaction, "saidas_producao")
                || !TableExists(connection, transaction, "saidas_producao_itens"))
            {
                return null;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT sp.numero,
                           CAST(sp.dt_movimento AS TEXT) AS dt_movimento,
                           COALESCE(sp.finalidade, '') AS finalidade,
                           COUNT(spi.material) AS total_itens
                    FROM saidas_producao sp
                    INNER JOIN saidas_producao_itens spi ON sp.numero = spi.numero AND spi.status = 'ATIVO'
                    INNER JOIN movimentos_estoque me ON spi.numero = me.documento_numero
                        AND me.documento_tipo = 'SAIDA_PRODUCAO'
                        AND me.produto_utilizado = @produto
                        AND me.status = 'ATIVO'
                    WHERE sp.status = 'ATIVO'
                    GROUP BY sp.numero, sp.dt_movimento, sp.finalidade
                    ORDER BY sp.dt_movimento DESC
                    LIMIT 5";
                command.Parameters.Add(CreateParameter(command, "@produto", productCode));

                using (var reader = command.ExecuteReader())
                {
                    var lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(string.Format(
                            "{0}. Saida: {1}\n   Data: {2}\n   Finalidade: {3}\n   Total de itens: {4}",
                            lines.Count + 1,
                            ReadString(reader, "numero"),
                            ReadString(reader, "dt_movimento"),
                            ReadString(reader, "finalidade"),
                            ReadInt(reader, "total_itens")));
                    }

                    if (lines.Count == 0)
                    {
                        return null;
                    }

                    var title = productCode + (string.IsNullOrWhiteSpace(productDescription) ? string.Empty : " - " + productDescription);
                    var message = "Nao e possivel inativar o produto '" + title + "'.\n";
                    if (!string.IsNullOrWhiteSpace(productType))
                    {
                        message += "Tipo: " + productType + "\n";
                    }

                    message += "\nEste produto foi utilizado nas seguintes saidas de producao:\n\n"
                        + string.Join("\n\n", lines.ToArray());

                    if (lines.Count >= 5)
                    {
                        message += "\n\n(Mostrando apenas as 5 ultimas saidas)";
                    }

                    return message + "\n\nProdutos com historico de movimentacoes nao podem ser inativados para manter a integridade do historico de producao.";
                }
            }
        }

        private sealed class ProductRecord
        {
            public string Code { get; set; }

            public string Description { get; set; }

            public string Type { get; set; }

            public string Status { get; set; }
        }
    }
}
