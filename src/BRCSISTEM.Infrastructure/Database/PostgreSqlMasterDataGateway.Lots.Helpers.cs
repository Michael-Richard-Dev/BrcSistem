using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlMasterDataGateway
    {
        private static string GenerateNextLotCodeLocked(DbConnection connection, DbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(MAX(CAST(SUBSTRING(codigo FROM 2) AS INTEGER)), 0) + 1
                    FROM lotes
                    WHERE codigo ~ '^L[0-9]+$'";
                var nextNumber = Convert.ToInt32(command.ExecuteScalar() ?? 1);
                return "L" + nextNumber.ToString("000", CultureInfo.InvariantCulture);
            }
        }

        private static LotRecord LoadLatestLot(DbConnection connection, DbTransaction transaction, string lotCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT codigo,
                           COALESCE(nome, '') AS nome,
                           COALESCE(material, '') AS material,
                           COALESCE(fornecedor, '') AS fornecedor,
                           COALESCE(validade, '') AS validade,
                           status
                    FROM lotes
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new LotRecord
                    {
                        Code = ReadString(reader, "codigo"),
                        Name = ReadString(reader, "nome"),
                        MaterialCode = ReadString(reader, "material"),
                        SupplierCode = ReadString(reader, "fornecedor"),
                        ExpirationDate = NormalizeLotValidityText(ReadString(reader, "validade")),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static List<LotDuplicateRecord> FindActiveLotDuplicatesByNameAndMaterial(DbConnection connection, DbTransaction transaction, string lotName, string materialCode, string currentLotCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT l.codigo,
                           COALESCE(l.nome, '') AS nome,
                           COALESCE(l.material, '') AS material,
                           COALESCE(l.fornecedor, '') AS fornecedor,
                           COALESCE(l.validade, '') AS validade,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(f.nome, '') AS fornecedor_nome
                    FROM lotes l
                    LEFT JOIN embalagens e ON e.codigo = l.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN fornecedores f ON f.codigo = l.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    WHERE UPPER(TRIM(COALESCE(l.nome, ''))) = UPPER(TRIM(COALESCE(@nome, '')))
                      AND COALESCE(l.material, '') = COALESCE(@material, '')
                      AND l.status = 'ATIVO'
                      AND (@codigo_atual IS NULL OR l.codigo <> @codigo_atual)
                      AND l.versao = (
                          SELECT MAX(versao)
                          FROM lotes x
                          WHERE x.codigo = l.codigo
                      )
                    ORDER BY l.codigo";
                command.Parameters.Add(CreateParameter(command, "@nome", lotName));
                command.Parameters.Add(CreateParameter(command, "@material", string.IsNullOrWhiteSpace(materialCode) ? (object)DBNull.Value : materialCode));
                command.Parameters.Add(CreateParameter(command, "@codigo_atual", string.IsNullOrWhiteSpace(currentLotCode) ? (object)DBNull.Value : currentLotCode));

                var items = new List<LotDuplicateRecord>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LotDuplicateRecord
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            ExpirationDate = NormalizeLotValidityText(ReadString(reader, "validade")),
                        });
                    }
                }

                return items;
            }
        }

        private static string BuildLotDuplicateSameMaterialMessage(string lotName, string materialCode, IEnumerable<LotDuplicateRecord> duplicates)
        {
            var duplicateList = duplicates == null ? new List<LotDuplicateRecord>() : duplicates.ToList();
            var first = duplicateList.FirstOrDefault();
            var materialText = materialCode;
            if (first != null && !string.IsNullOrWhiteSpace(first.MaterialDescription))
            {
                materialText = string.IsNullOrWhiteSpace(materialCode)
                    ? first.MaterialDescription
                    : materialCode + " - " + first.MaterialDescription;
            }

            var codes = string.Join(", ", duplicateList.Select(item => item.Code).Where(item => !string.IsNullOrWhiteSpace(item)).ToArray());
            return "Nao foi possivel salvar o lote.\n\n"
                + "Ja existe lote ativo com a mesma descricao para este material.\n\n"
                + "Material: " + (string.IsNullOrWhiteSpace(materialText) ? "-" : materialText) + "\n"
                + "Descricao do lote: " + lotName + "\n"
                + "Lote(s) ja cadastrado(s): " + (string.IsNullOrWhiteSpace(codes) ? "-" : codes) + "\n\n"
                + "Use a tela de alerta de lotes duplicados para acompanhar os casos existentes.";
        }

        private static string BuildLotStockIssueMessage(DbConnection connection, DbTransaction transaction, LotRecord current)
        {
            if (!TableExists(connection, transaction, "movimentos_estoque"))
            {
                return null;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT m.almoxarifado,
                           m.material,
                           COALESCE(e.descricao, m.material) AS material_desc,
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
                    WHERE m.lote = @codigo
                      AND m.status = 'ATIVO'
                    GROUP BY m.almoxarifado, m.material, e.descricao
                    HAVING SUM(
                        CASE
                            WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                            ELSE 0
                        END
                    ) > 0
                    ORDER BY m.almoxarifado, m.material
                    LIMIT 5";
                command.Parameters.Add(CreateParameter(command, "@codigo", current.Code));

                using (var reader = command.ExecuteReader())
                {
                    var lines = new List<string>();
                    while (reader.Read())
                    {
                        lines.Add(string.Format(
                            "{0}. Almoxarifado: {1}\n   Material: {2}\n   Saldo: {3:N2}",
                            lines.Count + 1,
                            ReadString(reader, "almoxarifado"),
                            ReadString(reader, "material_desc"),
                            ReadDecimal(reader, "saldo")));
                    }

                    if (lines.Count == 0)
                    {
                        return null;
                    }

                    var title = current.Code + (string.IsNullOrWhiteSpace(current.Name) ? string.Empty : " - " + current.Name);
                    var message = "Nao e possivel inativar o lote '" + title + "'.\n";
                    if (!string.IsNullOrWhiteSpace(current.SupplierCode))
                    {
                        message += "Fornecedor: " + current.SupplierCode + "\n";
                    }

                    return message
                        + "\nExistem movimentacoes deste lote com saldo em estoque:\n\n"
                        + string.Join("\n\n", lines.ToArray())
                        + "\n\nPara inativar este lote, primeiro e necessario zerar o saldo de todos os materiais vinculados a ele.";
                }
            }
        }

        private static string NormalizeLotValidityText(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            DateTime parsedDate;
            var formats = new[]
            {
                "dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
            };

            if (DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            var digits = new string(rawValue.Where(char.IsDigit).ToArray());
            if (digits.Length >= 8)
            {
                digits = digits.Substring(0, 8);
                return digits.Insert(2, "/").Insert(5, "/");
            }

            return rawValue.Trim();
        }

        private sealed class LotRecord
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public string MaterialCode { get; set; }

            public string SupplierCode { get; set; }

            public string ExpirationDate { get; set; }

            public string Status { get; set; }
        }

        private sealed class LotDuplicateRecord
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public string MaterialCode { get; set; }

            public string MaterialDescription { get; set; }

            public string SupplierCode { get; set; }

            public string SupplierName { get; set; }

            public string ExpirationDate { get; set; }
        }
    }
}
