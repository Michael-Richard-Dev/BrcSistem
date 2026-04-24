using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class LotManagementForm
    {
        private bool ValidateLotForCreate(SaveLotRequest request)
        {
            var duplicates = FindLotNameDuplicates(request.Name, null);
            var sameMaterial = duplicates.Where(item => string.Equals(item.MaterialCode ?? string.Empty, request.MaterialCode ?? string.Empty, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (sameMaterial.Length > 0)
            {
                ShowSameMaterialDuplicateMessage(request.Name, request.MaterialCode, sameMaterial);
                return false;
            }

            if (duplicates.Length == 0)
            {
                return true;
            }

            using (var dialog = new LotDuplicateConfirmationForm(request.Name, duplicates))
            {
                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        private bool ValidateLotForUpdate(SaveLotRequest request)
        {
            var duplicates = FindLotNameDuplicates(request.Name, request.Code);
            var sameMaterial = duplicates.Where(item => string.Equals(item.MaterialCode ?? string.Empty, request.MaterialCode ?? string.Empty, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (sameMaterial.Length > 0)
            {
                ShowSameMaterialDuplicateMessage(request.Name, request.MaterialCode, sameMaterial);
                return false;
            }

            return true;
        }

        private LotSummary[] FindLotNameDuplicates(string lotName, string currentCode)
        {
            var normalizedName = NormalizeLotNameForComparison(lotName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return new LotSummary[0];
            }

            return _allLots
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .Where(item => string.Equals(NormalizeLotNameForComparison(item.Name), normalizedName, StringComparison.OrdinalIgnoreCase))
                .Where(item => string.IsNullOrWhiteSpace(currentCode) || !string.Equals(item.Code, currentCode, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => ParseLotCode(item.Code))
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private void ShowSameMaterialDuplicateMessage(string lotName, string materialCode, LotSummary[] duplicates)
        {
            var first = duplicates.FirstOrDefault();
            var materialText = materialCode;
            if (first != null && !string.IsNullOrWhiteSpace(first.MaterialDescription))
            {
                materialText = string.IsNullOrWhiteSpace(materialCode)
                    ? first.MaterialDescription
                    : materialCode + " - " + first.MaterialDescription;
            }

            var codes = string.Join(", ", duplicates.Select(item => item.Code).Where(item => !string.IsNullOrWhiteSpace(item)).ToArray());
            var message = "Nao foi possivel salvar o lote.\n\n"
                + "Ja existe lote ativo com a mesma descricao para este material.\n\n"
                + "Material: " + (string.IsNullOrWhiteSpace(materialText) ? "-" : materialText) + "\n"
                + "Descricao do lote: " + lotName + "\n"
                + "Lote(s) ja cadastrado(s): " + (string.IsNullOrWhiteSpace(codes) ? "-" : codes) + "\n\n"
                + "Use a tela de alerta de lotes duplicados para acompanhar os casos existentes.";
            SetStatus("Duplicidade de lote detectada.", true);
            MessageBox.Show(this, message, "Lote Duplicado por Material", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OpenSupplierLookup()
        {
            using (var dialog = new SelecaoRegistroForm("Selecionar Fornecedor", "Nome", _supplierOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_supplierComboBox, _supplierOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenMaterialLookup()
        {
            using (var dialog = new SelecaoRegistroForm("Selecionar Material", "Descricao", _materialOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_materialComboBox, _materialOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenSupplierManagement()
        {
            var selectedSupplierCode = (_supplierComboBox.SelectedItem as LookupOption)?.Code;
            using (var dialog = new SupplierManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            LoadReferenceData();
            if (!string.IsNullOrWhiteSpace(selectedSupplierCode))
            {
                SelectOptionByCode(_supplierComboBox, _supplierOptions, selectedSupplierCode);
            }
        }

        private void ApplyPrefillSelections()
        {
            if (_prefillApplied)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(_prefillMaterialCode))
            {
                SelectOptionByCode(_materialComboBox, _materialOptions, ExtractCode(_prefillMaterialCode));
            }

            if (!string.IsNullOrWhiteSpace(_prefillSupplierCode))
            {
                SelectOptionByCode(_supplierComboBox, _supplierOptions, ExtractCode(_prefillSupplierCode));
            }

            _prefillApplied = true;
        }

        private LotSummary[] SortLots(LotSummary[] items)
        {
            Func<LotSummary, object> selector;
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "nome": selector = item => item.Name ?? string.Empty; break;
                case "material": selector = item => item.MaterialDisplay ?? string.Empty; break;
                case "fornecedor": selector = item => item.SupplierDisplay ?? string.Empty; break;
                case "validade": selector = item => item.ExpirationDate ?? string.Empty; break;
                case "status": selector = item => item.Status ?? string.Empty; break;
                case "versao": selector = item => item.Version; break;
                default: selector = item => ParseLotCode(item.Code); break;
            }

            return (_sortAscending ? items.OrderBy(selector) : items.OrderByDescending(selector))
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static void BindCombo(ComboBox comboBox, LookupOption[] options)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(options.Cast<object>().ToArray());
            comboBox.EndUpdate();
            comboBox.SelectedIndex = -1;
        }

        private static void SelectOptionByCode(ComboBox comboBox, LookupOption[] options, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            for (var index = 0; index < options.Length; index++)
            {
                if (string.Equals(options[index].Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }

            comboBox.SelectedIndex = -1;
        }

        private static void EnsureOptionPresent(ref LookupOption[] options, ComboBox comboBox, string code, string description)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            if (options.All(item => !string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase)))
            {
                options = options
                    .Concat(new[]
                    {
                        new LookupOption
                        {
                            Code = code,
                            Description = description,
                            Status = "INATIVO",
                        },
                    })
                    .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                BindCombo(comboBox, options);
            }

            SelectOptionByCode(comboBox, options, code);
        }

        private static string NormalizeExpirationInput(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).Take(8).ToArray());
            if (digits.Length > 2) digits = digits.Insert(2, "/");
            if (digits.Length > 5) digits = digits.Insert(5, "/");
            return digits;
        }

        private static string ExtractCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var parts = value.Split(new[] { " - " }, StringSplitOptions.None);
            return parts[0].Trim();
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

        private static string NormalizeLotNameForComparison(string value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Trim()
                .ToUpperInvariant();
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label { AutoSize = true, Text = text, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 8, 0, 4) };
        }

        private static Label CreateStatCaption(string text, Color color, int leftMargin)
        {
            return new Label { AutoSize = true, Text = text, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = color, Margin = new Padding(leftMargin, 8, 2, 0) };
        }

        private static Label CreateStatValue(Color color)
        {
            return new Label { AutoSize = true, Text = "0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = color, Margin = new Padding(0, 8, 12, 0) };
        }
    }
}
