using System;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class MasterDataService
    {
        private readonly IMasterDataGateway _masterDataGateway;
        private readonly IAuditTrailService _auditTrailService;

        public MasterDataService(IMasterDataGateway masterDataGateway, IAuditTrailService auditTrailService)
        {
            _masterDataGateway = masterDataGateway;
            _auditTrailService = auditTrailService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadSuppliers(profile, settings)
                .OrderBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void CreateSupplier(AppConfiguration configuration, DatabaseProfile profile, SaveSupplierRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeSupplierRequest(request);
            _masterDataGateway.CreateSupplier(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de fornecedor",
                $"Tela=CadastroFornecedor; Acao=Salvar; Depois=(codigo={normalized.Code}; nome={normalized.Name}; cidade={normalized.City}; habilitado_brc={normalized.IsBrcEnabled}; status={normalized.Status})",
                settings);
        }

        public void UpdateSupplier(AppConfiguration configuration, DatabaseProfile profile, SaveSupplierRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeSupplierRequest(request);
            _masterDataGateway.UpdateSupplier(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de fornecedor",
                $"Tela=CadastroFornecedor; Acao=Alterar; Depois=(codigo={normalized.Code}; nome={normalized.Name}; cidade={normalized.City}; habilitado_brc={normalized.IsBrcEnabled}; status={normalized.Status})",
                settings);
        }

        public void InactivateSupplier(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string supplierCode)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(supplierCode))
            {
                throw new InvalidOperationException("Selecione um fornecedor.");
            }

            var normalizedCode = NormalizeCode(supplierCode, "codigo do fornecedor");
            _masterDataGateway.InactivateSupplier(profile, settings, normalizedCode);
            SafeAudit(profile, actorUserName, "Inativacao de fornecedor",
                $"Tela=CadastroFornecedor; Acao=Inativar; Codigo={normalizedCode}",
                settings);
        }

        public PackagingSummary[] LoadPackagings(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadPackagings(profile, settings)
                .OrderBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void CreatePackaging(AppConfiguration configuration, DatabaseProfile profile, SavePackagingRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizePackagingRequest(request);
            _masterDataGateway.CreatePackaging(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de embalagem",
                $"Tela=CadastroEmbalagem; Acao=Salvar; Depois=(codigo={normalized.Code}; descricao={normalized.Description}; habilitado_brc={normalized.IsBrcEnabled}; status={normalized.Status})",
                settings);
        }

        public void UpdatePackaging(AppConfiguration configuration, DatabaseProfile profile, SavePackagingRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizePackagingRequest(request);
            _masterDataGateway.UpdatePackaging(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de embalagem",
                $"Tela=CadastroEmbalagem; Acao=Alterar; Depois=(codigo={normalized.Code}; descricao={normalized.Description}; habilitado_brc={normalized.IsBrcEnabled}; status={normalized.Status})",
                settings);
        }

        public void InactivatePackaging(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string packagingCode)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(packagingCode))
            {
                throw new InvalidOperationException("Selecione uma embalagem.");
            }

            var normalizedCode = NormalizeCode(packagingCode, "codigo da embalagem");
            _masterDataGateway.InactivatePackaging(profile, settings, normalizedCode);
            SafeAudit(profile, actorUserName, "Inativacao de embalagem",
                $"Tela=CadastroEmbalagem; Acao=Inativar; Codigo={normalizedCode}",
                settings);
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadWarehouses(profile, settings)
                .OrderBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void CreateWarehouse(AppConfiguration configuration, DatabaseProfile profile, SaveWarehouseRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeWarehouseRequest(request);
            _masterDataGateway.CreateWarehouse(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de almoxarifado",
                $"Tela=CadastroAlmoxarifado; Acao=Salvar; Depois=(codigo={normalized.Code}; nome={normalized.Name}; empresa={normalized.CompanyCode}; empresa_nome={normalized.CompanyName}; status={normalized.Status})",
                settings);
        }

        public void UpdateWarehouse(AppConfiguration configuration, DatabaseProfile profile, SaveWarehouseRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeWarehouseRequest(request);
            _masterDataGateway.UpdateWarehouse(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de almoxarifado",
                $"Tela=CadastroAlmoxarifado; Acao=Alterar; Depois=(codigo={normalized.Code}; nome={normalized.Name}; empresa={normalized.CompanyCode}; empresa_nome={normalized.CompanyName}; status={normalized.Status})",
                settings);
        }

        public void InactivateWarehouse(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string warehouseCode)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                throw new InvalidOperationException("Selecione um almoxarifado.");
            }

            var normalizedCode = NormalizeCode(warehouseCode, "codigo do almoxarifado");
            _masterDataGateway.InactivateWarehouse(profile, settings, normalizedCode);
            SafeAudit(profile, actorUserName, "Inativacao de almoxarifado",
                $"Tela=CadastroAlmoxarifado; Acao=Inativar; Codigo={normalizedCode}",
                settings);
        }

        private static SaveSupplierRequest NormalizeSupplierRequest(SaveSupplierRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Code = NormalizeCode(request.Code, "codigo do fornecedor");
            request.Name = NormalizeRequiredUpperText(request.Name, "nome do fornecedor");
            request.Cnpj = NormalizeCnpj(request.Cnpj);
            request.City = NormalizeUpperText(request.City);
            request.Status = NormalizeStatus(request.Status);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            return request;
        }

        private static SavePackagingRequest NormalizePackagingRequest(SavePackagingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Code = NormalizeCode(request.Code, "codigo da embalagem");
            request.Description = NormalizeRequiredUpperText(request.Description, "descricao da embalagem");
            request.Status = NormalizeStatus(request.Status);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            return request;
        }

        private static SaveWarehouseRequest NormalizeWarehouseRequest(SaveWarehouseRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Code = NormalizeCode(request.Code, "codigo do almoxarifado");
            request.Name = NormalizeRequiredUpperText(request.Name, "nome do almoxarifado");
            request.CompanyCode = NormalizeText(request.CompanyCode);
            request.CompanyName = NormalizeUpperText(request.CompanyName);
            request.Status = NormalizeStatus(request.Status);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            return request;
        }

        private static string NormalizeCode(string value, string fieldDescription)
        {
            var trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Length == 0)
            {
                throw new InvalidOperationException("Informe o " + fieldDescription + ".");
            }

            if (!trimmed.All(char.IsDigit))
            {
                throw new InvalidOperationException("O campo " + fieldDescription + " deve conter apenas numeros.");
            }

            return trimmed;
        }

        private static string NormalizeCnpj(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length != 14)
            {
                throw new InvalidOperationException("CNPJ invalido.");
            }

            if (digits.Distinct().Count() == 1)
            {
                throw new InvalidOperationException("CNPJ invalido.");
            }

            return digits;
        }

        private static string NormalizeRequiredUpperText(string value, string fieldDescription)
        {
            var normalized = NormalizeUpperText(value);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidOperationException("Informe o " + fieldDescription + ".");
            }

            return normalized;
        }

        private static string NormalizeUpperText(string value)
        {
            return NormalizeText(value).ToUpperInvariant();
        }

        private static string NormalizeText(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static string NormalizeStatus(string value)
        {
            var status = string.IsNullOrWhiteSpace(value) ? "ATIVO" : value.Trim().ToUpperInvariant();
            if (!string.Equals(status, "ATIVO", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(status, "INATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Status invalido.");
            }

            return status;
        }

        private static string NormalizeActor(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "sistema" : value.Trim();
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
