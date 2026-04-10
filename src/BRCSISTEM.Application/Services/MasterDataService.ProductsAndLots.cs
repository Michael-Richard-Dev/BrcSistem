using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed partial class MasterDataService
    {
        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadProducts(profile, settings)
                .OrderBy(item => ParseNumericCode(item.Code))
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void CreateProduct(AppConfiguration configuration, DatabaseProfile profile, SaveProductRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeProductRequest(request);
            _masterDataGateway.CreateProduct(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de produto",
                $"Tela=CadastroProduto; Acao=Salvar; Depois=(codigo={normalized.Code}; descricao={normalized.Description}; status={normalized.Status})",
                settings);
        }

        public void UpdateProduct(AppConfiguration configuration, DatabaseProfile profile, SaveProductRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeProductRequest(request);
            _masterDataGateway.UpdateProduct(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de produto",
                $"Tela=CadastroProduto; Acao=Alterar; Depois=(codigo={normalized.Code}; descricao={normalized.Description}; status={normalized.Status})",
                settings);
        }

        public void InactivateProduct(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string productCode)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(productCode))
            {
                throw new InvalidOperationException("Selecione um produto.");
            }

            var normalizedCode = NormalizeCode(productCode, "codigo do produto");
            _masterDataGateway.InactivateProduct(profile, settings, normalizedCode);
            SafeAudit(profile, actorUserName, "Inativacao de produto",
                $"Tela=CadastroProduto; Acao=Inativar; Codigo={normalizedCode}",
                settings);
        }

        public string GenerateNextLotCode(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.GenerateNextLotCode(profile, settings);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadLots(profile, settings)
                .OrderBy(item => ParseLotCode(item.Code))
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public string CreateLot(AppConfiguration configuration, DatabaseProfile profile, SaveLotRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeLotRequest(request, allowMissingCode: true);
            var createdCode = _masterDataGateway.CreateLot(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Cadastro de lote",
                $"Tela=CadastroLote; Acao=Salvar; Depois=(codigo={createdCode}; nome={normalized.Name}; material={normalized.MaterialCode}; fornecedor={normalized.SupplierCode}; validade={normalized.ExpirationDate}; status={normalized.Status})",
                settings);

            return createdCode;
        }

        public void UpdateLot(AppConfiguration configuration, DatabaseProfile profile, SaveLotRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeLotRequest(request, allowMissingCode: false);
            _masterDataGateway.UpdateLot(profile, settings, normalized);

            SafeAudit(profile, normalized.ActorUserName, "Alteracao de lote",
                $"Tela=CadastroLote; Acao=Alterar; Depois=(codigo={normalized.Code}; nome={normalized.Name}; material={normalized.MaterialCode}; fornecedor={normalized.SupplierCode}; validade={normalized.ExpirationDate}; status={normalized.Status})",
                settings);
        }

        public void InactivateLot(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string lotCode)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(lotCode))
            {
                throw new InvalidOperationException("Selecione um lote.");
            }

            var normalizedCode = NormalizeLotCode(lotCode, allowMissingCode: false);
            _masterDataGateway.InactivateLot(profile, settings, normalizedCode);
            SafeAudit(profile, actorUserName, "Inativacao de lote",
                $"Tela=CadastroLote; Acao=Inativar; Codigo={normalizedCode}",
                settings);
        }

        private static SaveProductRequest NormalizeProductRequest(SaveProductRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Code = NormalizeCode(request.Code, "codigo do produto");
            request.Description = NormalizeRequiredUpperText(request.Description, "descricao do produto");
            request.Status = NormalizeStatus(request.Status);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            return request;
        }

        private static SaveLotRequest NormalizeLotRequest(SaveLotRequest request, bool allowMissingCode)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Code = NormalizeLotCode(request.Code, allowMissingCode);
            request.Name = NormalizeRequiredUpperText(CollapseSpaces(request.Name), "nome do lote");
            request.MaterialCode = NormalizeReferenceCode(request.MaterialCode);
            request.SupplierCode = NormalizeCode(request.SupplierCode, "codigo do fornecedor");
            request.ExpirationDate = NormalizeExpirationDate(request.ExpirationDate);
            request.Status = NormalizeStatus(request.Status);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            return request;
        }

        private static string NormalizeLotCode(string value, bool allowMissingCode)
        {
            var trimmed = NormalizeText(value).ToUpperInvariant();
            if (trimmed.Length == 0)
            {
                if (allowMissingCode)
                {
                    return null;
                }

                throw new InvalidOperationException("Informe o codigo do lote.");
            }

            return trimmed;
        }

        private static string NormalizeReferenceCode(string value)
        {
            return NormalizeText(value);
        }

        private static string NormalizeExpirationDate(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length == 0)
            {
                throw new InvalidOperationException("Informe a data de validade do lote.");
            }

            if (digits.Length != 8)
            {
                throw new InvalidOperationException("Data invalida.");
            }

            var formatted = digits.Insert(2, "/").Insert(5, "/");
            DateTime expirationDate;
            if (!DateTime.TryParseExact(formatted, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expirationDate))
            {
                throw new InvalidOperationException("Data invalida.");
            }

            if (expirationDate.Date < DateTime.Today)
            {
                throw new InvalidOperationException("A validade nao pode ser menor que a data atual.");
            }

            return formatted;
        }

        private static string CollapseSpaces(string value)
        {
            var text = (value ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
            return string.Join(" ", text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static int ParseNumericCode(string code)
        {
            int parsed;
            return int.TryParse(code, out parsed) ? parsed : int.MaxValue;
        }

        private static int ParseLotCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return int.MaxValue;
            }

            var trimmed = code.Trim();
            if (trimmed.Length > 1 && (trimmed[0] == 'L' || trimmed[0] == 'l'))
            {
                int parsed;
                if (int.TryParse(trimmed.Substring(1), out parsed))
                {
                    return parsed;
                }
            }

            return int.MaxValue;
        }
    }
}
