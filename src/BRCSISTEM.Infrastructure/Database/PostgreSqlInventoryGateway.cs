using System;
using System.Collections.Generic;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway : IInventoryGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlInventoryGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public string GenerateNextInventoryNumber(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero
                    FROM inventarios
                    WHERE numero ~ '^INV[0-9]+$'
                    ORDER BY CAST(SUBSTRING(numero FROM 4) AS INTEGER) DESC
                    LIMIT 1";
                var result = command.ExecuteScalar();
                var current = result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
                if (string.IsNullOrWhiteSpace(current) || current.Length < 4)
                {
                    return "INV000001";
                }

                int sequence;
                return int.TryParse(current.Substring(3), out sequence)
                    ? "INV" + (sequence + 1).ToString("000000")
                    : "INV000001";
            }
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

        public IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            bool onlyBrc)
        {
            var items = new List<PackagingSummary>();
            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                return items;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT m.material,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.almoxarifado = @almoxarifado
                          AND (@data_movimento = '' OR m.data_movimento <= @data_movimento)
                        GROUP BY m.material
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT e.codigo,
                           COALESCE(e.descricao, '') AS descricao,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc,
                           e.status,
                           e.versao,
                           s.saldo
                    FROM saldos s
                    INNER JOIN embalagens e ON e.codigo = s.material
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM embalagens
                        GROUP BY codigo
                    ) ex ON ex.codigo = e.codigo AND ex.max_versao = e.versao
                    WHERE e.status = 'ATIVO'
                      AND (@somente_brc = FALSE OR COALESCE(e.habilitado_brc, FALSE) = TRUE)
                    ORDER BY COALESCE(e.descricao, ''), e.codigo";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@somente_brc", onlyBrc));

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
                            StockBalance = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string materialCode,
            string movementDateTime)
        {
            var items = new List<LotSummary>();
            if (string.IsNullOrWhiteSpace(warehouseCode) || string.IsNullOrWhiteSpace(materialCode))
            {
                return items;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT m.lote,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.almoxarifado = @almoxarifado
                          AND m.material = @material
                          AND (@data_movimento = '' OR m.data_movimento <= @data_movimento)
                        GROUP BY m.lote
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT l.codigo,
                           COALESCE(l.nome, '') AS nome,
                           COALESCE(l.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(l.fornecedor, '') AS fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(l.validade, '') AS validade,
                           l.status,
                           l.versao,
                           s.saldo
                    FROM saldos s
                    INNER JOIN lotes l ON l.codigo = s.lote
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
                      AND l.material = @material
                    ORDER BY COALESCE(l.validade, ''), COALESCE(l.nome, ''), l.codigo";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));

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
                            ExpirationDate = ReadString(reader, "validade"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            StockBalance = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<InventoryPlanningCandidateItem> LoadPlanningCandidatesByWarehouse(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            bool onlyBrc)
        {
            var items = new List<InventoryPlanningCandidateItem>();
            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                return items;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT m.material,
                               m.lote,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.almoxarifado = @almoxarifado
                          AND (@data_movimento = '' OR m.data_movimento <= @data_movimento)
                        GROUP BY m.material, m.lote
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT @almoxarifado AS almoxarifado,
                           s.material,
                           COALESCE(e.descricao, '') AS material_nome,
                           s.lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           s.saldo,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc
                    FROM saldos s
                    LEFT JOIN embalagens e ON e.codigo = s.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = s.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE (@somente_brc = FALSE OR COALESCE(e.habilitado_brc, FALSE) = TRUE)
                    ORDER BY COALESCE(e.descricao, ''), s.material, COALESCE(l.nome, ''), s.lote";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@somente_brc", onlyBrc));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryPlanningCandidateItem
                        {
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_nome"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            SystemBalance = ReadDecimal(reader, "saldo"),
                            IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                        });
                    }
                }
            }

            return items;
        }
    }
}
