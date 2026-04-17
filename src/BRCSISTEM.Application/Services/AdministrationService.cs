using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Catalog;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class AdministrationService
    {
        private readonly IAdministrationGateway _administrationGateway;
        private readonly IAuditTrailService _auditTrailService;
        private readonly PermissionCategory[] _permissionCategories;
        private readonly string[] _allPermissionKeys;

        public AdministrationService(
            IAdministrationGateway administrationGateway,
            IAuditTrailService auditTrailService)
        {
            _administrationGateway = administrationGateway;
            _auditTrailService = auditTrailService;
            _permissionCategories = LegacyPermissionCatalog.Create();
            _allPermissionKeys = LegacyPermissionCatalog.GetAllPermissionKeys();
        }

        public PermissionCategory[] GetPermissionCategories()
        {
            return _permissionCategories.ToArray();
        }

        public UserSummary[] LoadUsers(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _administrationGateway.LoadUsers(profile, settings)
                .OrderBy(item => item.UserName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public string[] LoadUserTypeNames(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _administrationGateway.LoadUserTypeNames(profile, settings)
                .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void CreateUser(AppConfiguration configuration, DatabaseProfile profile, SaveUserRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeUserRequest(request, true);
            _administrationGateway.CreateUser(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de usuario",
                $"Tela=CadastroUsuario; Acao=Salvar; Depois=(usuario={normalized.UserName}; nome={normalized.DisplayName}; tipo={normalized.UserType}; status={normalized.Status})",
                settings);
        }

        public void UpdateUser(AppConfiguration configuration, DatabaseProfile profile, SaveUserRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeUserRequest(request, false);
            _administrationGateway.UpdateUser(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de usuario",
                $"Tela=CadastroUsuario; Acao=Alterar; Depois=(usuario={normalized.OriginalUserName}; nome={normalized.DisplayName}; tipo={normalized.UserType}; status={normalized.Status})",
                settings);
        }

        public void InactivateUser(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string userName)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidOperationException("Selecione um usuario.");
            }

            _administrationGateway.InactivateUser(profile, settings, userName.Trim());
            SafeAudit(profile, actorUserName, "Inativacao de usuario",
                $"Tela=CadastroUsuario; Acao=Inativar; Usuario={userName.Trim()}",
                settings);
        }

        public UserTypeSummary[] LoadUserTypes(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _administrationGateway.LoadUserTypes(profile, settings, _allPermissionKeys.Length)
                .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public UserTypeDetail LoadUserType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            var settings = GetSettings(configuration, profile);
            var detail = _administrationGateway.LoadUserType(profile, settings, typeName);
            if (detail == null)
            {
                return null;
            }

            var keys = detail.PermissionKeys ?? Array.Empty<string>();
            if (keys.Contains("*", StringComparer.OrdinalIgnoreCase))
            {
                detail.PermissionKeys = _allPermissionKeys;
            }

            return detail;
        }

        public int CountActiveUsersForType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            var settings = GetSettings(configuration, profile);
            return _administrationGateway.CountActiveUsersForType(profile, settings, typeName);
        }

        public UserTypeSaveResult SaveUserType(AppConfiguration configuration, DatabaseProfile profile, SaveUserTypeRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeUserTypeRequest(request);
            var selectedKeys = normalized.PermissionKeys ?? Array.Empty<string>();
            var permissionsSerialized = selectedKeys.Count >= _allPermissionKeys.Length
                ? "*"
                : string.Join(",", selectedKeys.OrderBy(item => item, StringComparer.OrdinalIgnoreCase));

            var result = _administrationGateway.SaveUserType(profile, settings, normalized, permissionsSerialized);
            var action = result.IsNewRecord ? "Tipo cadastrado" : "Tipo atualizado";
            var details = result.IsNewRecord
                ? $"Tipo: {result.SavedName}"
                : $"Tipo: {result.SavedName} | Usuarios versionados: {result.UpdatedUsersCount}";

            SafeAudit(profile, normalized.ActorUserName, action, details, settings);
            return result;
        }

        public void DeleteUserType(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string typeName)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException("Selecione um tipo para excluir.");
            }

            _administrationGateway.DeleteUserType(profile, settings, typeName.Trim());
            SafeAudit(profile, actorUserName, "Tipo excluido", $"Tipo: {typeName.Trim()}", settings);
        }

        public UserSummary[] LoadUsersByType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return Array.Empty<UserSummary>();
            }

            return _administrationGateway.LoadUsersByType(profile, settings, typeName.Trim())
                .OrderBy(item => item.UserName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public AccessRequest[] LoadPendingAccessRequests(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            var items = _administrationGateway.LoadPendingAccessRequests(profile, settings);
            return items == null ? Array.Empty<AccessRequest>() : items.ToArray();
        }

        public AccessRequest LoadAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string requestId)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return null;
            }
            return _administrationGateway.LoadAccessRequest(profile, settings, requestId.Trim());
        }

        public void LogAccessManagementOpened(AppConfiguration configuration, DatabaseProfile profile, string actorUserName)
        {
            var settings = GetSettings(configuration, profile);
            SafeAudit(profile, actorUserName, "Tela aberta", "Gerenciar Acessos", settings);
        }

        public void ApproveAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string requestId)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new InvalidOperationException("Selecione uma solicitacao para aprovar.");
            }

            var trimmedId = requestId.Trim();
            var current = _administrationGateway.LoadAccessRequest(profile, settings, trimmedId);
            if (current == null)
            {
                throw new InvalidOperationException("Solicitacao nao encontrada.");
            }

            var actor = string.IsNullOrWhiteSpace(actorUserName) ? "sistema" : actorUserName.Trim();
            _administrationGateway.ApproveAccessRequest(profile, settings, trimmedId, actor, NowText());

            SafeAudit(profile, actor, "Solicitacao aprovada",
                $"Aprovada solicitacao de {current.Name} ({current.Email})",
                settings);
        }

        public void CancelAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string requestId)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new InvalidOperationException("Selecione uma solicitacao para cancelar.");
            }

            var trimmedId = requestId.Trim();
            var current = _administrationGateway.LoadAccessRequest(profile, settings, trimmedId);
            if (current == null)
            {
                throw new InvalidOperationException("Solicitacao nao encontrada.");
            }

            var actor = string.IsNullOrWhiteSpace(actorUserName) ? "sistema" : actorUserName.Trim();
            _administrationGateway.CancelAccessRequest(profile, settings, trimmedId, actor, NowText());

            SafeAudit(profile, actor, "Solicitacao cancelada",
                $"Cancelada solicitacao de {current.Name}",
                settings);
        }

        private static string NowText()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static SaveUserRequest NormalizeUserRequest(SaveUserRequest request, bool creating)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var userName = (request.UserName ?? string.Empty).Trim().ToLowerInvariant();
            var displayName = (request.DisplayName ?? string.Empty).Trim().ToUpperInvariant();
            var password = (request.Password ?? string.Empty).Trim();
            var userType = (request.UserType ?? string.Empty).Trim();
            var status = string.IsNullOrWhiteSpace(request.Status) ? "ATIVO" : request.Status.Trim().ToUpperInvariant();

            if (userName.Length < 3)
            {
                throw new InvalidOperationException("O campo Usuario e obrigatorio e deve ter pelo menos 3 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new InvalidOperationException("O campo Nome e obrigatorio.");
            }

            if (creating && password.Length < 6)
            {
                throw new InvalidOperationException("A senha e obrigatoria e deve ter pelo menos 6 caracteres.");
            }

            if (!creating && !string.IsNullOrWhiteSpace(password) && password.Length < 6)
            {
                throw new InvalidOperationException("A nova senha deve ter pelo menos 6 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(userType))
            {
                throw new InvalidOperationException("O campo Tipo de Usuario e obrigatorio.");
            }

            request.UserName = userName;
            request.OriginalUserName = string.IsNullOrWhiteSpace(request.OriginalUserName)
                ? userName
                : request.OriginalUserName.Trim().ToLowerInvariant();
            request.DisplayName = displayName;
            request.Password = password;
            request.UserType = userType;
            request.Status = status;
            request.ActorUserName = string.IsNullOrWhiteSpace(request.ActorUserName) ? userName : request.ActorUserName.Trim();
            return request;
        }

        private SaveUserTypeRequest NormalizeUserTypeRequest(SaveUserTypeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var name = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Informe o nome do tipo de usuario.");
            }

            var selectedKeys = (request.PermissionKeys ?? Array.Empty<string>())
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(key => key.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var key in selectedKeys)
            {
                if (!_allPermissionKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Foi encontrada uma permissao invalida: " + key);
                }
            }

            request.OriginalName = string.IsNullOrWhiteSpace(request.OriginalName) ? null : request.OriginalName.Trim();
            request.Name = name;
            request.Description = (request.Description ?? string.Empty).Trim();
            request.PermissionKeys = selectedKeys;
            request.ActorUserName = string.IsNullOrWhiteSpace(request.ActorUserName) ? "sistema" : request.ActorUserName.Trim();
            return request;
        }

        private static ConnectionResilienceSettings GetSettings(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                throw new InvalidOperationException("Banco de dados nao informado.");
            }

            configuration.Normalize();
            return configuration.ConnectionSettings ?? ConnectionResilienceSettings.CreateDefault();
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
