using System;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;
using BRCSISTEM.Domain.Security;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlBootstrapper : IDatabaseBootstrapper
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlBootstrapper(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void EnsureCoreSchema(DatabaseProfile profile, FirstUserSeed firstUser, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction())
            {
                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS usuarios (
                        usuario TEXT NOT NULL,
                        nome TEXT,
                        senha TEXT,
                        salt TEXT,
                        tipo TEXT DEFAULT 'Usuario',
                        status TEXT DEFAULT 'ATIVO',
                        versao INTEGER DEFAULT 1,
                        dt_hr_criacao TEXT,
                        dt_hr_alteracao TEXT,
                        PRIMARY KEY (usuario, versao)
                    )");

                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS tipos_usuario (
                        tipo TEXT PRIMARY KEY,
                        descricao TEXT,
                        permissoes TEXT
                    )");

                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS parametros (
                        id SERIAL PRIMARY KEY,
                        chave TEXT UNIQUE,
                        valor TEXT,
                        dt_hr_alteracao TEXT
                    )");

                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS logs_auditoria (
                        id SERIAL PRIMARY KEY,
                        usuario TEXT,
                        acao TEXT,
                        detalhes TEXT,
                        dt_hr TEXT
                    )");

                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS registro_bloqueios (
                        id SERIAL PRIMARY KEY,
                        tabela VARCHAR(100) NOT NULL,
                        registro_chave TEXT NOT NULL,
                        usuario VARCHAR(100) NOT NULL,
                        data_bloqueio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        data_liberacao TIMESTAMP NULL,
                        ativo BOOLEAN DEFAULT true,
                        observacoes TEXT,
                        dt_hr_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        dt_hr_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    )");

                ExecuteNonQuery(connection, transaction, @"
                    CREATE TABLE IF NOT EXISTS usuario_almoxarifados (
                        id SERIAL PRIMARY KEY,
                        usuario VARCHAR(100) NOT NULL,
                        codigo_almoxarifado TEXT NOT NULL,
                        dt_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        usuario_criacao VARCHAR(100),
                        UNIQUE(usuario, codigo_almoxarifado)
                    )");

                EnsureMasterDataSchema(connection, transaction);
                EnsureDefaultUserTypes(connection, transaction);
                EnsureDefaultParameters(connection, transaction);
                EnsureFirstUser(connection, transaction, firstUser);
                transaction.Commit();
            }
        }

        internal static DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        internal static int ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql, params DbParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                return command.ExecuteNonQuery();
            }
        }

        internal static object ExecuteScalar(DbConnection connection, DbTransaction transaction, string sql, params DbParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                return command.ExecuteScalar();
            }
        }

        private static void EnsureDefaultUserTypes(DbConnection connection, DbTransaction transaction)
        {
            if (ToInt(ExecuteScalar(connection, transaction, "SELECT COUNT(*) FROM tipos_usuario")) > 0)
            {
                return;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO tipos_usuario (tipo, descricao, permissoes) VALUES (@tipo, @descricao, @permissoes)";
                command.Parameters.Add(CreateParameter(command, "@tipo", "Administrador"));
                command.Parameters.Add(CreateParameter(command, "@descricao", "Administrador do sistema"));
                command.Parameters.Add(CreateParameter(command, "@permissoes", "*"));
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO tipos_usuario (tipo, descricao, permissoes) VALUES (@tipo, @descricao, @permissoes)";
                command.Parameters.Add(CreateParameter(command, "@tipo", "Usuario"));
                command.Parameters.Add(CreateParameter(command, "@descricao", "Usuario padrao"));
                command.Parameters.Add(CreateParameter(command, "@permissoes", string.Empty));
                command.ExecuteNonQuery();
            }
        }

        private static void EnsureDefaultParameters(DbConnection connection, DbTransaction transaction)
        {
            InsertParameterIfMissing(connection, transaction, "versao", "3.1.20");
            InsertParameterIfMissing(connection, transaction, "limite_dias_entrada", "7");
            InsertParameterIfMissing(connection, transaction, "limite_dias_transferencia", "7");
            InsertParameterIfMissing(connection, transaction, "limite_dias_saida", "7");
            InsertParameterIfMissing(connection, transaction, "data_fechamento", string.Empty);
            InsertParameterIfMissing(connection, transaction, "usar_peps", "SIM");
            InsertParameterIfMissing(connection, transaction, "inventario_cancelador_diferente_criador", "SIM");
        }

        private static void InsertParameterIfMissing(DbConnection connection, DbTransaction transaction, string key, string value)
        {
            using (var checkCommand = connection.CreateCommand())
            {
                checkCommand.Transaction = transaction;
                checkCommand.CommandText = "SELECT COUNT(*) FROM parametros WHERE chave = @chave";
                checkCommand.Parameters.Add(CreateParameter(checkCommand, "@chave", key));
                if (ToInt(checkCommand.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            using (var insertCommand = connection.CreateCommand())
            {
                insertCommand.Transaction = transaction;
                insertCommand.CommandText = "INSERT INTO parametros (chave, valor, dt_hr_alteracao) VALUES (@chave, @valor, @agora)";
                insertCommand.Parameters.Add(CreateParameter(insertCommand, "@chave", key));
                insertCommand.Parameters.Add(CreateParameter(insertCommand, "@valor", value));
                insertCommand.Parameters.Add(CreateParameter(insertCommand, "@agora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                insertCommand.ExecuteNonQuery();
            }
        }

        private static void EnsureFirstUser(DbConnection connection, DbTransaction transaction, FirstUserSeed firstUser)
        {
            if (firstUser == null || !firstUser.HasValues)
            {
                return;
            }

            if (ToInt(ExecuteScalar(connection, transaction, "SELECT COUNT(*) FROM usuarios WHERE status = 'ATIVO'")) > 0)
            {
                return;
            }

            var salt = Guid.NewGuid().ToString();
            var hash = PasswordHasher.HashSha256(firstUser.Password, salt);
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO usuarios (usuario, nome, senha, salt, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                    VALUES (@usuario, @nome, @senha, @salt, @tipo, @status, @versao, @criacao, @alteracao)";
                command.Parameters.Add(CreateParameter(command, "@usuario", firstUser.UserName));
                command.Parameters.Add(CreateParameter(command, "@nome", firstUser.Name));
                command.Parameters.Add(CreateParameter(command, "@senha", hash));
                command.Parameters.Add(CreateParameter(command, "@salt", salt));
                command.Parameters.Add(CreateParameter(command, "@tipo", "Administrador"));
                command.Parameters.Add(CreateParameter(command, "@status", "ATIVO"));
                command.Parameters.Add(CreateParameter(command, "@versao", 1));
                command.Parameters.Add(CreateParameter(command, "@criacao", now));
                command.Parameters.Add(CreateParameter(command, "@alteracao", now));
                command.ExecuteNonQuery();
            }
        }

        private static void EnsureMasterDataSchema(DbConnection connection, DbTransaction transaction)
        {
            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS produtos (
                    codigo TEXT NOT NULL,
                    descricao TEXT,
                    tipo TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (codigo, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE produtos ADD COLUMN IF NOT EXISTS tipo TEXT");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS fornecedores (
                    codigo TEXT NOT NULL,
                    nome TEXT,
                    cnpj TEXT,
                    cidade TEXT,
                    habilitado_brc BOOLEAN DEFAULT FALSE,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (codigo, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE fornecedores ADD COLUMN IF NOT EXISTS cidade TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE fornecedores ADD COLUMN IF NOT EXISTS habilitado_brc BOOLEAN DEFAULT FALSE");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS embalagens (
                    codigo TEXT NOT NULL,
                    descricao TEXT,
                    unidade TEXT,
                    habilitado_brc BOOLEAN DEFAULT FALSE,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (codigo, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE embalagens ADD COLUMN IF NOT EXISTS unidade TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE embalagens ADD COLUMN IF NOT EXISTS habilitado_brc BOOLEAN DEFAULT FALSE");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS almoxarifados (
                    codigo TEXT NOT NULL,
                    nome TEXT,
                    empresa TEXT,
                    empresa_nome TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (codigo, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE almoxarifados ADD COLUMN IF NOT EXISTS empresa TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE almoxarifados ADD COLUMN IF NOT EXISTS empresa_nome TEXT");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS lotes (
                    codigo TEXT NOT NULL,
                    nome TEXT,
                    material TEXT,
                    fornecedor TEXT,
                    validade TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (codigo, versao)
                )");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS notas (
                    numero TEXT NOT NULL,
                    fornecedor TEXT NOT NULL,
                    almoxarifado TEXT,
                    dt_emissao TEXT,
                    dt_movimento TEXT,
                    usuario TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    bloqueado_por TEXT,
                    bloqueado_em TEXT,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (numero, fornecedor, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE notas ADD COLUMN IF NOT EXISTS bloqueado_por TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE notas ADD COLUMN IF NOT EXISTS bloqueado_em TEXT");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS notas_itens (
                    numero TEXT NOT NULL,
                    material TEXT NOT NULL,
                    fornecedor TEXT NOT NULL,
                    lote TEXT NOT NULL,
                    almoxarifado TEXT NOT NULL,
                    quantidade DECIMAL,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (numero, fornecedor, material, lote, almoxarifado, versao)
                )");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS transferencias (
                    numero TEXT NOT NULL,
                    dt_movimento TEXT,
                    almox_origem TEXT,
                    almox_destino TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    bloqueado_por TEXT,
                    bloqueado_em TEXT,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (numero, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE transferencias ADD COLUMN IF NOT EXISTS bloqueado_por TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE transferencias ADD COLUMN IF NOT EXISTS bloqueado_em TEXT");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS transferencias_itens (
                    id SERIAL PRIMARY KEY,
                    numero TEXT NOT NULL,
                    item_numero INTEGER,
                    material TEXT NOT NULL,
                    lote TEXT NOT NULL,
                    quantidade DECIMAL,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE transferencias_itens ADD COLUMN IF NOT EXISTS item_numero INTEGER");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS saidas_producao (
                    numero TEXT NOT NULL,
                    finalidade TEXT,
                    dt_movimento TEXT,
                    turno TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    bloqueado_por TEXT,
                    bloqueado_em TEXT,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (numero, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao ADD COLUMN IF NOT EXISTS turno TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao ADD COLUMN IF NOT EXISTS bloqueado_por TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao ADD COLUMN IF NOT EXISTS bloqueado_em TEXT");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS saidas_producao_itens (
                    numero TEXT NOT NULL,
                    produto TEXT,
                    material TEXT NOT NULL,
                    lote TEXT NOT NULL,
                    almoxarifado TEXT NOT NULL,
                    quantidade DECIMAL,
                    qtd_envio DECIMAL,
                    qtd_retorno DECIMAL,
                    qtd_consumida DECIMAL,
                    status TEXT DEFAULT 'ATIVO',
                    versao INTEGER DEFAULT 1,
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT,
                    PRIMARY KEY (numero, material, lote, almoxarifado, versao)
                )");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao_itens ADD COLUMN IF NOT EXISTS produto TEXT");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao_itens ADD COLUMN IF NOT EXISTS qtd_envio DECIMAL");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao_itens ADD COLUMN IF NOT EXISTS qtd_retorno DECIMAL");
            ExecuteNonQuery(connection, transaction, "ALTER TABLE saidas_producao_itens ADD COLUMN IF NOT EXISTS qtd_consumida DECIMAL");

            ExecuteNonQuery(connection, transaction, @"
                CREATE TABLE IF NOT EXISTS movimentos_estoque (
                    id SERIAL PRIMARY KEY,
                    documento_numero TEXT,
                    documento_tipo TEXT,
                    documento_item INTEGER,
                    data_movimento TEXT,
                    tipo TEXT,
                    fornecedor TEXT,
                    almoxarifado TEXT,
                    material TEXT,
                    lote TEXT,
                    quantidade DECIMAL,
                    produto_utilizado TEXT,
                    vencimento TEXT,
                    usuario TEXT,
                    status TEXT DEFAULT 'ATIVO',
                    dt_hr_criacao TEXT,
                    dt_hr_alteracao TEXT
                )");

            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_fornecedores_codigo ON fornecedores(codigo)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_fornecedores_cnpj ON fornecedores(cnpj)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_embalagens_codigo ON embalagens(codigo)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_produtos_codigo ON produtos(codigo)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_almoxarifados_codigo ON almoxarifados(codigo)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_lotes_codigo ON lotes(codigo)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_lotes_material ON lotes(material)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_lotes_fornecedor ON lotes(fornecedor)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_notas_numero_fornecedor ON notas(numero, fornecedor)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_notas_status ON notas(status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_notas_bloqueado_por ON notas(bloqueado_por)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_notas_itens_numero_fornecedor ON notas_itens(numero, fornecedor)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_notas_itens_material_lote ON notas_itens(material, lote)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_transferencias_numero ON transferencias(numero)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_transferencias_status ON transferencias(status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_transferencias_bloqueado_por ON transferencias(bloqueado_por)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_transferencias_itens_numero ON transferencias_itens(numero)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_transferencias_itens_material_lote ON transferencias_itens(material, lote)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_saidas_producao_numero ON saidas_producao(numero)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_saidas_producao_status ON saidas_producao(status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_saidas_producao_bloqueado_por ON saidas_producao(bloqueado_por)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_saidas_producao_itens_numero ON saidas_producao_itens(numero)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_saidas_producao_itens_material_lote ON saidas_producao_itens(material, lote)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_material_status ON movimentos_estoque(material, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_lote_status ON movimentos_estoque(lote, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_almoxarifado_status ON movimentos_estoque(almoxarifado, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_produto_utilizado_status ON movimentos_estoque(produto_utilizado, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_documento_nota ON movimentos_estoque(documento_numero, documento_tipo, fornecedor, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_documento_transferencia ON movimentos_estoque(documento_numero, documento_tipo, status)");
            ExecuteNonQuery(connection, transaction, "CREATE INDEX IF NOT EXISTS idx_movimentos_documento_saida_producao ON movimentos_estoque(documento_numero, documento_tipo, status)");
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }
    }
}
