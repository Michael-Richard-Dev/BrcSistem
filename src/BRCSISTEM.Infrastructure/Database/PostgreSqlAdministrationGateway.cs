using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;
using BRCSISTEM.Domain.Security;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlAdministrationGateway : IAdministrationGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlAdministrationGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<UserSummary> LoadUsers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var users = new List<UserSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT u1.usuario, u1.nome, u1.tipo, u1.status, u1.versao
                    FROM usuarios u1
                    INNER JOIN (
                        SELECT usuario, MAX(versao) AS max_versao
                        FROM usuarios
                        GROUP BY usuario
                    ) u2 ON u1.usuario = u2.usuario AND u1.versao = u2.max_versao
                    ORDER BY u1.usuario";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserSummary
                        {
                            UserName = ReadString(reader, "usuario"),
                            DisplayName = ReadString(reader, "nome"),
                            UserType = ReadString(reader, "tipo"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return users;
        }

        public IReadOnlyCollection<string> LoadUserTypeNames(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<string>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT tipo FROM tipos_usuario ORDER BY tipo";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var value = ReadString(reader, "tipo");
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            items.Add(value);
                        }
                    }
                }
            }

            return items;
        }

        public void CreateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE usuarios IN EXCLUSIVE MODE");

                    if (!UserTypeExists(connection, transaction, request.UserType))
                    {
                        throw new InvalidOperationException(
                            "Tipo de usuario '" + request.UserType + "' nao existe ou nao possui permissoes configuradas.");
                    }

                    if (ActiveUserExists(connection, transaction, request.UserName))
                    {
                        throw new InvalidOperationException("O usuario '" + request.UserName + "' ja esta cadastrado como ATIVO.");
                    }

                    var nextVersion = GetNextUserVersion(connection, transaction, request.UserName);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO usuarios (usuario, nome, senha, salt, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@usuario, @nome, @senha, @salt, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@usuario", request.UserName));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.DisplayName));
                        command.Parameters.Add(CreateParameter(command, "@senha", PasswordHasher.HashSha256(request.Password, null)));
                        command.Parameters.Add(CreateParameter(command, "@salt", DBNull.Value));
                        command.Parameters.Add(CreateParameter(command, "@tipo", request.UserType));
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

        public void UpdateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserRequest request)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE usuarios IN EXCLUSIVE MODE");

                    if (!UserTypeExists(connection, transaction, request.UserType))
                    {
                        throw new InvalidOperationException(
                            "Tipo de usuario '" + request.UserType + "' nao existe ou nao possui permissoes configuradas.");
                    }

                    var current = LoadLatestUserInternal(connection, transaction, request.OriginalUserName);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Usuario nao encontrado.");
                    }

                    var nextVersion = GetNextUserVersion(connection, transaction, request.OriginalUserName);
                    var now = NowText();
                    var keepCurrentPassword = string.IsNullOrWhiteSpace(request.Password);
                    var passwordHash = keepCurrentPassword
                        ? current.PasswordHash
                        : PasswordHasher.HashSha256(request.Password, null);
                    var salt = keepCurrentPassword ? current.Salt : null;

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO usuarios (usuario, nome, senha, salt, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@usuario, @nome, @senha, @salt, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@usuario", request.OriginalUserName));
                        command.Parameters.Add(CreateParameter(command, "@nome", request.DisplayName));
                        command.Parameters.Add(CreateParameter(command, "@senha", passwordHash));
                        command.Parameters.Add(CreateParameter(command, "@salt", string.IsNullOrWhiteSpace(salt) ? (object)DBNull.Value : salt));
                        command.Parameters.Add(CreateParameter(command, "@tipo", request.UserType));
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

        public void InactivateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE usuarios IN EXCLUSIVE MODE");
                    var current = LoadLatestUserInternal(connection, transaction, userName);
                    if (current == null)
                    {
                        throw new InvalidOperationException("Usuario nao encontrado.");
                    }

                    var nextVersion = GetNextUserVersion(connection, transaction, userName);
                    var now = NowText();
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO usuarios (usuario, nome, senha, salt, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                            VALUES (@usuario, @nome, @senha, @salt, @tipo, @status, @versao, @criacao, @alteracao)";
                        command.Parameters.Add(CreateParameter(command, "@usuario", current.UserName));
                        command.Parameters.Add(CreateParameter(command, "@nome", current.DisplayName));
                        command.Parameters.Add(CreateParameter(command, "@senha", current.PasswordHash));
                        command.Parameters.Add(CreateParameter(command, "@salt", string.IsNullOrWhiteSpace(current.Salt) ? (object)DBNull.Value : current.Salt));
                        command.Parameters.Add(CreateParameter(command, "@tipo", current.UserType));
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

        public IReadOnlyCollection<UserTypeSummary> LoadUserTypes(DatabaseProfile profile, ConnectionResilienceSettings settings, int totalPermissionCount)
        {
            var items = new List<UserTypeSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT tipo, descricao, permissoes
                    FROM tipos_usuario
                    ORDER BY tipo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rawPermissions = ReadString(reader, "permissoes");
                        var count = string.Equals(rawPermissions, "*", StringComparison.OrdinalIgnoreCase)
                            ? totalPermissionCount
                            : SplitPermissions(rawPermissions).Length;

                        items.Add(new UserTypeSummary
                        {
                            Name = ReadString(reader, "tipo"),
                            Description = ReadString(reader, "descricao"),
                            PermissionCount = count,
                            TotalPermissionCount = totalPermissionCount,
                        });
                    }
                }
            }

            return items;
        }

        public UserTypeDetail LoadUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT tipo, descricao, permissoes FROM tipos_usuario WHERE tipo = @tipo";
                command.Parameters.Add(CreateParameter(command, "@tipo", typeName));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserTypeDetail
                    {
                        Name = ReadString(reader, "tipo"),
                        Description = ReadString(reader, "descricao"),
                        PermissionKeys = SplitPermissions(ReadString(reader, "permissoes")),
                    };
                }
            }
        }

        public int CountActiveUsersForType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(DISTINCT usuario) AS total
                    FROM usuarios
                    WHERE tipo = @tipo
                      AND versao = (
                          SELECT MAX(versao)
                          FROM usuarios u2
                          WHERE u2.usuario = usuarios.usuario
                      )
                      AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@tipo", typeName));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0);
            }
        }

        public UserTypeSaveResult SaveUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserTypeRequest request, string permissionsSerialized)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE tipos_usuario IN EXCLUSIVE MODE");
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE usuarios IN EXCLUSIVE MODE");

                    if (string.IsNullOrWhiteSpace(request.OriginalName))
                    {
                        if (UserTypeNameExists(connection, transaction, request.Name, null))
                        {
                            throw new InvalidOperationException("Ja existe um tipo de usuario com o nome '" + request.Name + "'.");
                        }

                        using (var insert = connection.CreateCommand())
                        {
                            insert.Transaction = transaction;
                            insert.CommandText = @"
                                INSERT INTO tipos_usuario (tipo, descricao, permissoes)
                                VALUES (@tipo, @descricao, @permissoes)";
                            insert.Parameters.Add(CreateParameter(insert, "@tipo", request.Name));
                            insert.Parameters.Add(CreateParameter(insert, "@descricao", request.Description));
                            insert.Parameters.Add(CreateParameter(insert, "@permissoes", permissionsSerialized));
                            insert.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return new UserTypeSaveResult
                        {
                            SavedName = request.Name,
                            IsNewRecord = true,
                            UpdatedUsersCount = 0,
                        };
                    }

                    var original = LoadOriginalUserType(connection, transaction, request.OriginalName);
                    if (original == null)
                    {
                        throw new InvalidOperationException("O registro nao existe mais.");
                    }

                    if (UserTypeNameExists(connection, transaction, request.Name, request.OriginalName))
                    {
                        throw new InvalidOperationException("Ja existe um tipo de usuario com o nome '" + request.Name + "'.");
                    }

                    var nameChanged = !string.Equals(request.Name, original.Name, StringComparison.Ordinal);
                    var descriptionChanged = !string.Equals(request.Description ?? string.Empty, original.Description ?? string.Empty, StringComparison.Ordinal);

                    using (var update = connection.CreateCommand())
                    {
                        update.Transaction = transaction;
                        update.CommandText = @"
                            UPDATE tipos_usuario
                            SET tipo = @novo_tipo,
                                descricao = @descricao,
                                permissoes = @permissoes
                            WHERE tipo = @tipo_original";
                        update.Parameters.Add(CreateParameter(update, "@novo_tipo", request.Name));
                        update.Parameters.Add(CreateParameter(update, "@descricao", request.Description));
                        update.Parameters.Add(CreateParameter(update, "@permissoes", permissionsSerialized));
                        update.Parameters.Add(CreateParameter(update, "@tipo_original", request.OriginalName));
                        update.ExecuteNonQuery();
                    }

                    var updatedUsersCount = 0;
                    if (nameChanged || descriptionChanged)
                    {
                        using (var updateUsers = connection.CreateCommand())
                        {
                            updateUsers.Transaction = transaction;
                            updateUsers.CommandText = @"
                                WITH usuarios_afetados AS (
                                    SELECT usuario, MAX(versao) AS versao_atual
                                    FROM usuarios
                                    WHERE tipo = @tipo_original
                                    GROUP BY usuario
                                )
                                INSERT INTO usuarios (usuario, nome, senha, salt, tipo, status, versao, dt_hr_criacao, dt_hr_alteracao)
                                SELECT u.usuario, u.nome, u.senha, u.salt, @novo_tipo, u.status, ua.versao_atual + 1, u.dt_hr_criacao, @alteracao
                                FROM usuarios u
                                INNER JOIN usuarios_afetados ua ON u.usuario = ua.usuario AND u.versao = ua.versao_atual
                                WHERE u.tipo = @tipo_original";
                            updateUsers.Parameters.Add(CreateParameter(updateUsers, "@tipo_original", request.OriginalName));
                            updateUsers.Parameters.Add(CreateParameter(updateUsers, "@novo_tipo", request.Name));
                            updateUsers.Parameters.Add(CreateParameter(updateUsers, "@alteracao", NowText()));
                            updatedUsersCount = updateUsers.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return new UserTypeSaveResult
                    {
                        SavedName = request.Name,
                        IsNewRecord = false,
                        UpdatedUsersCount = updatedUsersCount,
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void DeleteUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    ExecuteNonQuery(connection, transaction, "LOCK TABLE tipos_usuario IN EXCLUSIVE MODE");
                    var usersUsingType = CountUsersUsingType(connection, transaction, typeName);
                    if (usersUsingType > 0)
                    {
                        throw new InvalidOperationException(
                            "Nao e possivel excluir o tipo '" + typeName + "'. Existem " + usersUsingType + " usuario(s) utilizando este tipo.");
                    }

                    using (var delete = connection.CreateCommand())
                    {
                        delete.Transaction = transaction;
                        delete.CommandText = "DELETE FROM tipos_usuario WHERE tipo = @tipo";
                        delete.Parameters.Add(CreateParameter(delete, "@tipo", typeName));
                        var affected = delete.ExecuteNonQuery();
                        if (affected == 0)
                        {
                            throw new InvalidOperationException("Nenhum registro foi excluido.");
                        }
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

        public IReadOnlyCollection<UserSummary> LoadUsersByType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName)
        {
            var items = new List<UserSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT u.usuario, u.nome, u.status, u.versao
                    FROM usuarios u
                    WHERE u.tipo = @tipo
                      AND u.versao = (
                          SELECT MAX(versao)
                          FROM usuarios u2
                          WHERE u2.usuario = u.usuario
                      )
                    ORDER BY u.usuario";
                command.Parameters.Add(CreateParameter(command, "@tipo", typeName));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new UserSummary
                        {
                            UserName = ReadString(reader, "usuario"),
                            DisplayName = ReadString(reader, "nome"),
                            UserType = typeName,
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        private static bool UserTypeExists(DbConnection connection, DbTransaction transaction, string userType)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT 1 FROM tipos_usuario WHERE LOWER(tipo) = LOWER(@tipo)";
                command.Parameters.Add(CreateParameter(command, "@tipo", userType));
                return command.ExecuteScalar() != null;
            }
        }

        private static bool ActiveUserExists(DbConnection connection, DbTransaction transaction, string userName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT 1 FROM usuarios WHERE LOWER(usuario) = LOWER(@usuario) AND status = 'ATIVO'";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                return command.ExecuteScalar() != null;
            }
        }

        private static int GetNextUserVersion(DbConnection connection, DbTransaction transaction, string userName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COALESCE(MAX(versao), 0) + 1 FROM usuarios WHERE usuario = @usuario";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                return Convert.ToInt32(command.ExecuteScalar() ?? 1);
            }
        }

        private static UserAccount LoadLatestUserInternal(DbConnection connection, DbTransaction transaction, string userName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT usuario, COALESCE(nome, usuario) AS nome, senha, salt, tipo, status
                    FROM usuarios
                    WHERE LOWER(usuario) = LOWER(@usuario)
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserAccount
                    {
                        UserName = ReadString(reader, "usuario"),
                        DisplayName = ReadString(reader, "nome"),
                        PasswordHash = ReadString(reader, "senha"),
                        Salt = ReadString(reader, "salt"),
                        UserType = ReadString(reader, "tipo"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        private static bool UserTypeNameExists(DbConnection connection, DbTransaction transaction, string name, string originalName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(*)
                    FROM tipos_usuario
                    WHERE LOWER(tipo) = LOWER(@tipo)
                      AND (@tipo_original IS NULL OR LOWER(tipo) <> LOWER(@tipo_original))";
                command.Parameters.Add(CreateParameter(command, "@tipo", name));
                command.Parameters.Add(CreateParameter(command, "@tipo_original", string.IsNullOrWhiteSpace(originalName) ? (object)DBNull.Value : originalName));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0) > 0;
            }
        }

        private static UserTypeSummary LoadOriginalUserType(DbConnection connection, DbTransaction transaction, string typeName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT tipo, descricao FROM tipos_usuario WHERE tipo = @tipo";
                command.Parameters.Add(CreateParameter(command, "@tipo", typeName));
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserTypeSummary
                    {
                        Name = ReadString(reader, "tipo"),
                        Description = ReadString(reader, "descricao"),
                    };
                }
            }
        }

        private static int CountUsersUsingType(DbConnection connection, DbTransaction transaction, string typeName)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM usuarios WHERE tipo = @tipo";
                command.Parameters.Add(CreateParameter(command, "@tipo", typeName));
                return Convert.ToInt32(command.ExecuteScalar() ?? 0);
            }
        }

        private static string[] SplitPermissions(string rawPermissions)
        {
            if (string.IsNullOrWhiteSpace(rawPermissions))
            {
                return Array.Empty<string>();
            }

            if (string.Equals(rawPermissions.Trim(), "*", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { "*" };
            }

            return rawPermissions
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => item.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
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

        private static int ReadInt(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static string NowText()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
