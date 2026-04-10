using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;
using BRCSISTEM.Domain.Security;

namespace BRCSISTEM.Application.Services
{
    public sealed class AuthenticationService
    {
        private static readonly string[] DefaultPasswords = { "123456", "admin", "password", "123" };

        private readonly IDatabaseBootstrapper _databaseBootstrapper;
        private readonly IAuthenticationGateway _authenticationGateway;
        private readonly IAuditTrailService _auditTrailService;

        public AuthenticationService(
            IDatabaseBootstrapper databaseBootstrapper,
            IAuthenticationGateway authenticationGateway,
            IAuditTrailService auditTrailService)
        {
            _databaseBootstrapper = databaseBootstrapper;
            _authenticationGateway = authenticationGateway;
            _auditTrailService = auditTrailService;
        }

        public LoginResult Authenticate(AppConfiguration configuration, string profileId, string userName, string password)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Normalize();
            var profile = configuration.GetProfile(profileId);
            if (profile == null)
            {
                return LoginResult.Fail("Banco de dados selecionado nao foi encontrado.");
            }

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return LoginResult.Fail("Informe usuario e senha.");
            }

            _databaseBootstrapper.EnsureCoreSchema(profile, configuration.GetEffectiveFirstUser(), configuration.ConnectionSettings);

            var account = _authenticationGateway.FindUser(profile, userName.Trim(), configuration.ConnectionSettings);
            if (account == null)
            {
                SafeAudit(profile, userName, "Login falhou", "Usuario nao encontrado.", configuration.ConnectionSettings);
                return LoginResult.Fail("Usuario nao encontrado.");
            }

            var currentHash = PasswordHasher.HashSha256(password, account.Salt);
            var legacyHash = PasswordHasher.HashLegacyMd5(password);
            var loginValid = string.Equals(account.PasswordHash, currentHash, StringComparison.OrdinalIgnoreCase)
                || string.Equals(account.PasswordHash, legacyHash, StringComparison.OrdinalIgnoreCase);

            if (!loginValid)
            {
                SafeAudit(profile, userName, "Login falhou", "Usuario ou senha invalidos.", configuration.ConnectionSettings);
                return LoginResult.Fail("Usuario ou senha incorretos.");
            }

            if (!string.Equals(account.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                SafeAudit(profile, userName, "Login negado", "Usuario inativo.", configuration.ConnectionSettings);
                return LoginResult.Fail("Usuario inativo. Contate o administrador.");
            }

            var permissions = _authenticationGateway.LoadPermissionKeys(profile, account.UserType, configuration.ConnectionSettings);
            var normalizedPermissions = NormalizePermissions(permissions);
            var isAdministrator = string.Equals(account.UserType, "Administrador", StringComparison.OrdinalIgnoreCase)
                || normalizedPermissions.Contains("*", StringComparer.OrdinalIgnoreCase);

            if (!isAdministrator && normalizedPermissions.Count == 0)
            {
                SafeAudit(profile, userName, "Login negado", "Tipo de usuario sem permissoes configuradas.", configuration.ConnectionSettings);
                return LoginResult.Fail("O tipo de usuario nao possui permissoes validas para acessar o sistema.");
            }

            var identity = new UserIdentity
            {
                UserName = account.UserName,
                DisplayName = string.IsNullOrWhiteSpace(account.DisplayName) ? account.UserName : account.DisplayName,
                UserType = account.UserType,
                PermissionKeys = normalizedPermissions,
            };

            if (DefaultPasswords.Contains(password, StringComparer.OrdinalIgnoreCase))
            {
                SafeAudit(profile, userName, "Login senha padrao", "Troca de senha obrigatoria.", configuration.ConnectionSettings);
                return LoginResult.Successful(identity, profile, true);
            }

            SafeAudit(profile, userName, "Login realizado", "Login bem-sucedido.", configuration.ConnectionSettings);
            return LoginResult.Successful(identity, profile, false);
        }

        public PasswordChangeResult ChangePassword(AppConfiguration configuration, DatabaseProfile profile, string userName, string newPassword)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                return PasswordChangeResult.Fail("Banco de dados nao informado.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                return PasswordChangeResult.Fail("Usuario invalido.");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Trim().Length < 6)
            {
                return PasswordChangeResult.Fail("A nova senha precisa ter ao menos 6 caracteres.");
            }

            var salt = Guid.NewGuid().ToString();
            var hash = PasswordHasher.HashSha256(newPassword.Trim(), salt);
            _authenticationGateway.UpdatePassword(profile, userName.Trim(), hash, salt, configuration.ConnectionSettings);
            SafeAudit(profile, userName, "Senha alterada", "Senha atualizada com sucesso.", configuration.ConnectionSettings);
            return PasswordChangeResult.Ok("Senha alterada com sucesso.");
        }

        private static IReadOnlyCollection<string> NormalizePermissions(IReadOnlyCollection<string> permissions)
        {
            if (permissions == null)
            {
                return Array.Empty<string>();
            }

            return permissions
                .Where(permission => !string.IsNullOrWhiteSpace(permission))
                .Select(permission => permission.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private void SafeAudit(DatabaseProfile profile, string userName, string action, string details, ConnectionResilienceSettings settings)
        {
            try
            {
                _auditTrailService.Write(profile, userName, action, details, settings);
            }
            catch
            {
            }
        }
    }
}
