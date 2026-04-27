using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class SystemParametersForm
    {
        // ── Aba Geral ──────────────────────────────────────────────────────────
        private TextBox _versionTextBox;

        // ── Aba Travas ─────────────────────────────────────────────────────────
        private TextBox  _inboundLimitTextBox;
        private TextBox  _transferLimitTextBox;
        private TextBox  _outboundLimitTextBox;
        private TextBox  _closingDateTextBox;
        private ComboBox _inventoryCancellerRuleCombo;

        // Cache dos parametros carregados do banco (chave => valor)
        private Dictionary<string, string> _systemParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private void BuildGeneralTab(TabPage page)
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Configuracoes Gerais",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12),
            };

            var line = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
            line.Controls.Add(CreateFieldLabel("Versao do Sistema:"));
            _versionTextBox = new TextBox { Width = 180, ReadOnly = true, Font = new Font("Segoe UI", 10F) };
            line.Controls.Add(_versionTextBox);
            group.Controls.Add(line);

            var info = new GroupBox
            {
                Dock = DockStyle.Bottom,
                Text = "Informacoes do Sistema",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Padding(10),
                Height  = 100,
            };
            info.Controls.Add(new Label
            {
                Dock      = DockStyle.Fill,
                Text      = "- Versao atual do BRCSISTEM\r\n"
                          + "- Para atualizar o sistema, utilize a distribuicao na pasta \"DISTRIBUICAO\"\r\n"
                          + "- Documentacao: consulte a pasta \"docs\"",
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
            });
            group.Controls.Add(info);
            page.Controls.Add(group);
        }

        private void BuildLocksTab(TabPage page)
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Travas de Movimento",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12),
            };

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            root.Controls.Add(new Label
            {
                AutoSize  = true,
                Text      = "Configure limites de retroatividade, fechamento contabil e regras de inventario.",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Margin    = new Padding(0, 0, 0, 12),
            });

            _inboundLimitTextBox  = new TextBox { Width = 80,  Font = new Font("Segoe UI", 10F) };
            _transferLimitTextBox = new TextBox { Width = 80,  Font = new Font("Segoe UI", 10F) };
            _outboundLimitTextBox = new TextBox { Width = 80,  Font = new Font("Segoe UI", 10F) };
            _closingDateTextBox   = new TextBox { Width = 120, Font = new Font("Segoe UI", 10F) };
            _inventoryCancellerRuleCombo = new ComboBox
            {
                Width         = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
            };
            _inventoryCancellerRuleCombo.Items.AddRange(new object[] { "SIM", "NAO" });

            root.Controls.Add(BuildLockRow("Limite de dias para Entrada:",        _inboundLimitTextBox,  "(dias ou 0 para desativar)"));
            root.Controls.Add(BuildLockRow("Limite de dias para Transferencia:",  _transferLimitTextBox, "(dias ou 0 para desativar)"));
            root.Controls.Add(BuildLockRow("Limite de dias para Saida:",          _outboundLimitTextBox, "(dias ou 0 para desativar)"));
            root.Controls.Add(BuildLockRow("Data de Fechamento Contabil:",        _closingDateTextBox,   "(dd/mm/aaaa, 9999 ou deixe vazio)"));
            root.Controls.Add(BuildLockRow("Inventario - cancelador diferente do criador:", _inventoryCancellerRuleCombo, "(SIM = exige usuario diferente)"));

            var explanation = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Explicacao das Travas",
                Font    = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Padding(10),
                Height  = 95,
                Margin  = new Padding(0, 15, 0, 0),
            };
            explanation.Controls.Add(new Label
            {
                Dock      = DockStyle.Fill,
                Text      = "- Limite de dias: quantidade de dias para permitir movimento retroativo.\r\n"
                          + "- Data de Fechamento: bloqueia movimentos anteriores a data informada.\r\n"
                          + "- Cancelador diferente do criador: quando SIM, o usuario criador nao pode cancelar.",
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
            });
            root.Controls.Add(explanation);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 15, 0, 0) };
            buttons.Controls.Add(CreateButton("Salvar Configuracoes", (sender, args) => SaveSystemParameters()));
            root.Controls.Add(buttons);

            group.Controls.Add(root);
            page.Controls.Add(group);
        }

        private static FlowLayoutPanel BuildLockRow(string labelText, Control input, string helpText)
        {
            var row = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
            var label = new Label
            {
                AutoSize  = false,
                Width     = 280,
                Text      = labelText,
                Font      = new Font("Segoe UI", 9.5F),
                Margin    = new Padding(0, 10, 6, 4),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            row.Controls.Add(label);
            input.Margin = new Padding(0, 6, 6, 4);
            row.Controls.Add(input);
            row.Controls.Add(new Label
            {
                AutoSize  = true,
                Text      = helpText,
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Margin    = new Padding(0, 10, 6, 4),
            });
            return row;
        }

        private void ReloadSystemParameters()
        {
            var parameters = _databaseMaintenanceController.LoadSystemParameters(_configuration, _databaseProfile);

            _systemParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    if (!string.IsNullOrWhiteSpace(p?.Key))
                    {
                        _systemParameters[p.Key] = p.Value ?? string.Empty;
                    }
                }
            }

            _versionTextBox.Text = GetParameter("versao", "3.1.20");
            _inboundLimitTextBox.Text = GetParameter("limite_dias_entrada", "7");
            _transferLimitTextBox.Text = GetParameter("limite_dias_transferencia", "7");
            _outboundLimitTextBox.Text = GetParameter("limite_dias_saida", "7");
            _closingDateTextBox.Text = GetParameter("data_fechamento", string.Empty);

            var rule = GetParameter("inventario_cancelador_diferente_criador", "SIM").Trim().ToUpperInvariant();
            if (rule != "SIM" && rule != "NAO")
            {
                rule = "SIM";
            }

            _inventoryCancellerRuleCombo.SelectedItem = rule;
        }

        private string GetParameter(string key, string defaultValue)
        {
            string value;
            return _systemParameters.TryGetValue(key, out value) && value != null
                ? value
                : defaultValue;
        }

        private void SaveSystemParameters()
        {
            try
            {
                var errors  = new List<string>();
                var inbound = (_inboundLimitTextBox.Text  ?? string.Empty).Trim();
                var transfr = (_transferLimitTextBox.Text ?? string.Empty).Trim();
                var outbnd  = (_outboundLimitTextBox.Text ?? string.Empty).Trim();
                var closing = (_closingDateTextBox.Text   ?? string.Empty).Trim();
                var rule    = (_inventoryCancellerRuleCombo.SelectedItem as string ?? string.Empty).Trim().ToUpperInvariant();

                ValidateIntegerDays("Limite Dias Entrada",       inbound, errors);
                ValidateIntegerDays("Limite Dias Transferencia", transfr, errors);
                ValidateIntegerDays("Limite Dias Saida",         outbnd,  errors);

                if (!string.IsNullOrEmpty(closing)
                    && !string.Equals(closing, "9999", StringComparison.Ordinal)
                    && !IsValidBrazilianDate(closing))
                {
                    errors.Add("Data de Fechamento Contabil: formato invalido (use dd/mm/aaaa, 9999 ou vazio)");
                }

                if (rule != "SIM" && rule != "NAO")
                {
                    errors.Add("Inventario - cancelador diferente do criador: use SIM ou NAO");
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show(this,
                        "Corrija os seguintes erros:\r\n\r\n- " + string.Join("\r\n- ", errors),
                        "Erro de Validacao", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var actor = _identity.UserName;
                _databaseMaintenanceController.SaveSystemParameter(_configuration, _databaseProfile, actor, "limite_dias_entrada",        inbound);
                _databaseMaintenanceController.SaveSystemParameter(_configuration, _databaseProfile, actor, "limite_dias_transferencia",  transfr);
                _databaseMaintenanceController.SaveSystemParameter(_configuration, _databaseProfile, actor, "limite_dias_saida",          outbnd);
                _databaseMaintenanceController.SaveSystemParameter(_configuration, _databaseProfile, actor, "data_fechamento",            closing);
                _databaseMaintenanceController.SaveSystemParameter(_configuration, _databaseProfile, actor, "inventario_cancelador_diferente_criador", rule);

                ReloadSystemParameters();
                MessageBox.Show(this, "Parametros salvos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private static void ValidateIntegerDays(string fieldLabel, string value, List<string> errors)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            int days;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out days))
            {
                errors.Add(fieldLabel + ": deve ser numero inteiro");
                return;
            }

            if (days < 0)
            {
                errors.Add(fieldLabel + ": deve ser numero positivo (ou 0 para desativar)");
            }
        }

        private static bool IsValidBrazilianDate(string value)
        {
            DateTime parsed;
            return DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }
    }
}
