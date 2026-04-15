using System;
using System.Data.Common;
using System.Globalization;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway
    {
        private static InventoryHeaderRecord LoadLatestInventoryHeader(DbConnection connection, DbTransaction transaction, string number)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT i.numero,
                           COALESCE(i.status, '') AS status,
                           COALESCE(i.usuario, '') AS usuario,
                           COALESCE(i.observacao, '') AS observacao,
                           COALESCE(i.max_pontos, 1) AS max_pontos,
                           COALESCE(i.versao, 0) AS versao,
                           COALESCE(i.bloqueado_por, '') AS bloqueado_por,
                           COALESCE(i.dt_hr_criacao, '') AS dt_hr_criacao,
                           COALESCE(i.dt_inicio, '') AS dt_inicio,
                           COALESCE(i.dt_abertura, '') AS dt_abertura,
                           COALESCE(i.dt_fechamento, '') AS dt_fechamento,
                           COALESCE(i.dt_finalizacao, '') AS dt_finalizacao,
                           COALESCE(i.fechado_por, '') AS fechado_por,
                           COALESCE(i.cancelado_por, '') AS cancelado_por,
                           COALESCE(i.motivo_cancelamento, '') AS motivo_cancelamento
                    FROM inventarios i
                    WHERE i.numero = @numero
                    ORDER BY i.versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new InventoryHeaderRecord
                    {
                        Number = ReadString(reader, "numero"),
                        Status = ReadString(reader, "status"),
                        CreatedBy = ReadString(reader, "usuario"),
                        Observation = ReadString(reader, "observacao"),
                        MaxOpenPoints = ReadInt(reader, "max_pontos"),
                        Version = ReadInt(reader, "versao"),
                        LockedBy = ReadString(reader, "bloqueado_por"),
                        CreatedAt = ReadString(reader, "dt_hr_criacao"),
                        ScheduledAt = ReadString(reader, "dt_inicio"),
                        OpenedAt = ReadString(reader, "dt_abertura"),
                        ClosedAt = ReadString(reader, "dt_fechamento"),
                        FinalizedAt = ReadString(reader, "dt_finalizacao"),
                        ClosedBy = ReadString(reader, "fechado_por"),
                        CanceledBy = ReadString(reader, "cancelado_por"),
                        CancellationReason = ReadString(reader, "motivo_cancelamento"),
                    };
                }
            }
        }

        private static void ReleaseLockInternal(DbConnection connection, DbTransaction transaction, string number, string userName, bool updateHeader)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE registro_bloqueios
                       SET ativo = FALSE,
                           data_liberacao = CURRENT_TIMESTAMP,
                           dt_hr_alteracao = CURRENT_TIMESTAMP
                     WHERE tabela = 'inventarios'
                       AND registro_chave = @chave
                       AND ativo = TRUE
                       AND (@usuario IS NULL OR UPPER(usuario) = UPPER(@usuario))";
                command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number)));
                command.Parameters.Add(CreateParameter(command, "@usuario", string.IsNullOrWhiteSpace(userName) ? (object)DBNull.Value : userName));
                command.ExecuteNonQuery();
            }

            if (!updateHeader)
            {
                return;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE inventarios
                       SET bloqueado_por = NULL,
                           bloqueado_em = NULL,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND versao = (
                           SELECT MAX(versao)
                           FROM inventarios x
                           WHERE x.numero = @numero
                       )";
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.ExecuteNonQuery();
            }
        }

        private static void RecalculateCounts(DbConnection connection, DbTransaction transaction, string number)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    WITH somas AS (
                        SELECT almoxarifado,
                               material,
                               lote,
                               COALESCE(SUM(quantidade), 0) AS quantidade
                        FROM inventario_contagens
                        WHERE numero = @numero
                          AND status = 'ATIVO'
                        GROUP BY almoxarifado, material, lote
                    )
                    UPDATE inventarios_itens ii
                       SET quantidade_contada = s.quantidade,
                           ajuste = s.quantidade - COALESCE(ii.saldo_sistema, 0),
                           tipo_ajuste = CASE
                               WHEN s.quantidade - COALESCE(ii.saldo_sistema, 0) > 0 THEN 'ENTRADA'
                               WHEN s.quantidade - COALESCE(ii.saldo_sistema, 0) < 0 THEN 'REQUISICAO'
                               ELSE 'SEM_AJUSTE'
                           END,
                           dt_hr_alteracao = @agora
                      FROM somas s
                     WHERE ii.numero = @numero
                       AND ii.status = 'ATIVO'
                       AND ii.almoxarifado = s.almoxarifado
                       AND ii.material = s.material
                       AND ii.lote = s.lote";
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE inventarios_itens ii
                       SET quantidade_contada = NULL,
                           ajuste = NULL,
                           tipo_ajuste = NULL,
                           dt_hr_alteracao = @agora
                     WHERE ii.numero = @numero
                       AND ii.status = 'ATIVO'
                       AND NOT EXISTS (
                           SELECT 1
                           FROM inventario_contagens c
                           WHERE c.numero = ii.numero
                             AND c.status = 'ATIVO'
                             AND c.almoxarifado = ii.almoxarifado
                             AND c.material = ii.material
                             AND c.lote = ii.lote
                       )";
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.ExecuteNonQuery();
            }
        }

        private static decimal GetStockBalanceAtInternal(DbConnection connection, DbTransaction transaction, string warehouseCode, string materialCode, string lotCode, string movementDateTime)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
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
                      AND almoxarifado = @almoxarifado
                      AND material = @material
                      AND lote = @lote
                      AND (@data_movimento = '' OR data_movimento <= @data_movimento)";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value
                    ? 0M
                    : Convert.ToDecimal(result, CultureInfo.InvariantCulture);
            }
        }

        private static string GetLotSupplier(DbConnection connection, DbTransaction transaction, string lotCode, string materialCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(fornecedor, '')
                    FROM lotes
                    WHERE codigo = @codigo
                      AND (@material = '' OR material = @material)
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode ?? string.Empty));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
            }
        }

        private static string GetLotExpiration(DbConnection connection, DbTransaction transaction, string lotCode, string materialCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(validade, '')
                    FROM lotes
                    WHERE codigo = @codigo
                      AND (@material = '' OR material = @material)
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode ?? string.Empty));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
            }
        }

        private static string BuildLockKey(string number)
        {
            return "numero=" + number;
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
            return reader.IsDBNull(ordinal) ? 0M : Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static string NowText()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private sealed class InventoryHeaderRecord
        {
            public string Number { get; set; }

            public string Status { get; set; }

            public string CreatedBy { get; set; }

            public string Observation { get; set; }

            public int MaxOpenPoints { get; set; }

            public int Version { get; set; }

            public string LockedBy { get; set; }

            public string CreatedAt { get; set; }

            public string ScheduledAt { get; set; }

            public string OpenedAt { get; set; }

            public string ClosedAt { get; set; }

            public string FinalizedAt { get; set; }

            public string ClosedBy { get; set; }

            public string CanceledBy { get; set; }

            public string CancellationReason { get; set; }
        }
    }
}
