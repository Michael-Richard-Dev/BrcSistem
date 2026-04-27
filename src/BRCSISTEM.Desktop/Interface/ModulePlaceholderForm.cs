using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed class ModulePlaceholderForm : Form
    {
        public ModulePlaceholderForm(ModuleDefinition module, DatabaseProfile profile)
        {
            Text = "Modulo em Migracao";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(640, 360);
            MinimumSize = new Size(640, 360);

            var message = new Label
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Text = $"Modulo: {module.Title}\n\nGrupo: {module.Group}\nPermissao: {(string.IsNullOrWhiteSpace(module.RequiredPermission) ? "(sem permissao especifica)" : module.RequiredPermission)}\nBanco ativo: {profile.DisplayName}\nArquivo Python de origem: {module.PythonFile}\n\nDescricao:\n{module.Description}\n\nStatus atual:\nEste modulo ainda nao foi portado integralmente para C#. O shell WinForms foi preparado para encaixar a implementacao real sem perder o mapa funcional do sistema.",
            };

            var closeButton = new Button
            {
                Text = "Fechar",
                Dock = DockStyle.Bottom,
                Height = 42,
                FlatStyle = FlatStyle.System,
            };
            closeButton.Click += (sender, args) => Close();

            Controls.Add(message);
            Controls.Add(closeButton);
        }
    }
}
