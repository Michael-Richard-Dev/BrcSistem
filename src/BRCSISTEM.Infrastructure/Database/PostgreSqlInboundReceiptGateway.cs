using System;
using System.Collections.Generic;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInboundReceiptGateway : IInboundReceiptGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlInboundReceiptGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<PackagingSummary> LoadMaterialsBySupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode)
        {
            var items = new List<PackagingSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT e.codigo,
                           COALESCE(e.descricao, '') AS descricao,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc,
                           e.status,
                           e.versao
                    FROM lotes l
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM lotes
                        GROUP BY codigo
                    ) lx ON lx.codigo = l.codigo AND lx.max_versao = l.versao
                    INNER JOIN embalagens e ON e.codigo = l.material
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM embalagens
                        GROUP BY codigo
                    ) ex ON ex.codigo = e.codigo AND ex.max_versao = e.versao
                    WHERE l.status = 'ATIVO'
                      AND e.status = 'ATIVO'
                      AND l.fornecedor = @fornecedor
                    ORDER BY descricao, e.codigo";
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));

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
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName)
        {
            var items = new List<WarehouseSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                bool hasRestrictions;
                using (var restrictionCommand = connection.CreateCommand())
                {
                    restrictionCommand.CommandText = "SELECT COUNT(*) FROM usuario_almoxarifados WHERE UPPER(usuario) = UPPER(@usuario)";
                    restrictionCommand.Parameters.Add(CreateParameter(restrictionCommand, "@usuario", userName));
                    hasRestrictions = Convert.ToInt32(restrictionCommand.ExecuteScalar() ?? 0) > 0;
                }

                command.CommandText = hasRestrictions
                    ? @"
                        SELECT a.codigo,
                               COALESCE(a.nome, '') AS nome,
                               COALESCE(a.empresa, '') AS empresa,
                               COALESCE(a.empresa_nome, '') AS empresa_nome,
                               a.status,
                               a.versao
                        FROM almoxarifados a
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM almoxarifados
                            GROUP BY codigo
                        ) ax ON ax.codigo = a.codigo AND ax.max_versao = a.versao
                        INNER JOIN usuario_almoxarifados ua ON ua.codigo_almoxarifado = a.codigo
                        WHERE a.status = 'ATIVO'
                          AND UPPER(ua.usuario) = UPPER(@usuario)
                        ORDER BY a.nome, a.codigo"
                    : @"
                        SELECT a.codigo,
                               COALESCE(a.nome, '') AS nome,
                               COALESCE(a.empresa, '') AS empresa,
                               COALESCE(a.empresa_nome, '') AS empresa_nome,
                               a.status,
                               a.versao
                        FROM almoxarifados a
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM almoxarifados
                            GROUP BY codigo
                        ) ax ON ax.codigo = a.codigo AND ax.max_versao = a.versao
                        WHERE a.status = 'ATIVO'
                        ORDER BY a.nome, a.codigo";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));

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

        public IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode, string materialCode)
        {
            var items = new List<LotSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT l.codigo,
                           COALESCE(l.nome, '') AS nome,
                           COALESCE(l.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(l.fornecedor, '') AS fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(l.validade, '') AS validade,
                           l.status,
                           l.versao
                    FROM lotes l
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM lotes
                        GROUP BY codigo
                    ) lx ON lx.codigo = l.codigo AND lx.max_versao = l.versao
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
                    WHERE l.status = 'ATIVO'
                      AND (@fornecedor = '' OR l.fornecedor = @fornecedor)
                      AND (@material = '' OR l.material = @material)
                    ORDER BY l.nome, l.codigo";
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode ?? string.Empty));

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
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<InboundReceiptSummary> SearchReceipts(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter)
        {
            var items = new List<InboundReceiptSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var digitsFilter = DigitsOnly(filter);
                command.CommandText = @"
                    SELECT n.numero,
                           n.fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(n.almoxarifado, '') AS almoxarifado,
                           COALESCE(a.nome, '') AS almoxarifado_nome,
                           COALESCE(n.dt_movimento, '') AS dt_movimento,
                           COALESCE(n.status, '') AS status,
                           COALESCE(n.versao, 0) AS versao,
                           COALESCE(n.bloqueado_por, '') AS bloqueado_por
                    FROM notas n
                    INNER JOIN (
                        SELECT numero, fornecedor, MAX(versao) AS max_versao
                        FROM notas
                        GROUP BY numero, fornecedor
                    ) nx ON nx.numero = n.numero AND nx.fornecedor = n.fornecedor AND nx.max_versao = n.versao
                    LEFT JOIN fornecedores f ON f.codigo = n.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    LEFT JOIN almoxarifados a ON a.codigo = n.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    WHERE (@texto = ''
                        OR n.numero LIKE @texto_numero
                        OR n.fornecedor LIKE @texto_numero
                        OR COALESCE(f.nome, '') ILIKE @texto_like)
                    ORDER BY n.dt_movimento DESC, n.numero DESC";
                command.Parameters.Add(CreateParameter(command, "@texto", filter ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@texto_numero", string.IsNullOrWhiteSpace(digitsFilter) ? "__SEM_DIGITOS__" : "%" + digitsFilter + "%"));
                command.Parameters.Add(CreateParameter(command, "@texto_like", "%" + (filter ?? string.Empty) + "%"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InboundReceiptSummary
                        {
                            Number = ReadString(reader, "numero"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            WarehouseName = ReadString(reader, "almoxarifado_nome"),
                            MovementDateTime = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                        });
                    }
                }
            }

            return items;
        }

        public InboundReceiptDetail LoadReceiptDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            {
                var header = LoadLatestNoteHeader(connection, null, number, supplierCode);
                if (header == null)
                {
                    return null;
                }

                var items = new List<InboundReceiptItemDetail>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ni.material,
                               COALESCE(e.descricao, '') AS material_desc,
                               ni.lote,
                               COALESCE(l.nome, '') AS lote_nome,
                               COALESCE(ni.quantidade, 0) AS quantidade,
                               COALESCE(ni.status, '') AS status
                        FROM notas_itens ni
                        LEFT JOIN embalagens e ON e.codigo = ni.material
                            AND e.versao = (
                                SELECT MAX(versao)
                                FROM embalagens x
                                WHERE x.codigo = e.codigo
                            )
                        LEFT JOIN lotes l ON l.codigo = ni.lote
                            AND l.versao = (
                                SELECT MAX(versao)
                                FROM lotes x
                                WHERE x.codigo = l.codigo
                            )
                        WHERE ni.numero = @numero
                          AND ni.fornecedor = @fornecedor
                          AND ni.versao = @versao
                        ORDER BY UPPER(COALESCE(e.descricao, ni.material)), UPPER(COALESCE(l.nome, ni.lote)), ni.material, ni.lote";
                    command.Parameters.Add(CreateParameter(command, "@numero", number));
                    command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                    command.Parameters.Add(CreateParameter(command, "@versao", header.Version));

                    using (var reader = command.ExecuteReader())
                    {
                        var itemNumber = 0;
                        while (reader.Read())
                        {
                            itemNumber++;
                            items.Add(new InboundReceiptItemDetail
                            {
                                ItemNumber = itemNumber,
                                MaterialCode = ReadString(reader, "material"),
                                MaterialDescription = ReadString(reader, "material_desc"),
                                LotCode = ReadString(reader, "lote"),
                                LotName = ReadString(reader, "lote_nome"),
                                Quantity = ReadDecimal(reader, "quantidade"),
                                Status = ReadString(reader, "status"),
                            });
                        }
                    }
                }

                return new InboundReceiptDetail
                {
                    Number = header.Number,
                    SupplierCode = header.SupplierCode,
                    SupplierName = header.SupplierName,
                    WarehouseCode = header.WarehouseCode,
                    WarehouseName = header.WarehouseName,
                    EmissionDate = NormalizeDateText(header.EmissionDate),
                    ReceiptDateTime = NormalizeDateTimeText(header.MovementDateTime),
                    Status = header.Status,
                    Version = header.Version,
                    LockedBy = header.LockedBy,
                    Items = items.ToArray(),
                };
            }
        }

        public bool IsMaterialBrcEnabled(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COALESCE(habilitado_brc, FALSE)
                    FROM embalagens
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", materialCode));
                return Convert.ToBoolean(command.ExecuteScalar() ?? false);
            }
        }

        public decimal GetActiveStockBalance(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode, string lotCode, string warehouseCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COALESCE(SUM(
                        CASE
                            WHEN tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN quantidade
                            WHEN tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -quantidade
                            ELSE 0
                        END
                    ), 0)
                    FROM movimentos_estoque
                    WHERE status = 'ATIVO'
                      AND material = @material
                      AND lote = @lote
                      AND almoxarifado = @almoxarifado";
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                return Convert.ToDecimal(command.ExecuteScalar() ?? 0M);
            }
        }

        public string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COALESCE(valor, '') FROM parametros WHERE chave = @chave LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@chave", key));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(result))
                    ? defaultValue
                    : Convert.ToString(result);
            }
        }
    }
}
