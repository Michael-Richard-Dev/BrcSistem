using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Desktop.Interface;
using BRCSISTEM.Desktop.Interface.EditorPerfilBancoDados;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.PerfisBancoDados
{
    public sealed partial class PerfisBancoDadosForm : Form
    {
        private static readonly Color ActiveRowColor = Color.FromArgb(232, 245, 233);

        private readonly CompositionRoot _compositionRoot;
        private readonly ConfigurationController _configurationController;

        private AppConfiguration _configuration;
        private bool _hasChanges;
        private DatabaseManualForm _manualForm;

        public PerfisBancoDadosForm(CompositionRoot compositionRoot)
        {
            _compositionRoot = compositionRoot;
            _configurationController = compositionRoot.CreateConfigurationController();
            InitializeComponent();
            WireEvents();
        }

        private void WireEvents()
        {
            Load += PerfisBancoDadosForm_Load;
            _profilesListView.DoubleClick += (s, e) => EditButton_Click(s, EventArgs.Empty);
            _searchButton.Click += SearchButton_Click;
            _newButton.Click += NewButton_Click;
            _editButton.Click += EditButton_Click;
            _deleteButton.Click += DeleteSelectedProfile;
            _activateButton.Click += ActivateSelectedProfile;
            _createDatabaseButton.Click += CreateDatabaseButton_Click;
            _dropDatabaseButton.Click += DropDatabaseButton_Click;
            _manualButton.Click += ManualButton_Click;
            _closeButton.Click += CloseButton_Click;
            KeyPreview = true;
            KeyDown += PerfisBancoDadosForm_KeyDown;
        }

        private void PerfisBancoDadosForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void PerfisBancoDadosForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2: NewButton_Click(sender, EventArgs.Empty); e.Handled = true; break;
                case Keys.F3: EditButton_Click(sender, EventArgs.Empty); e.Handled = true; break;
                case Keys.F4: CloseButton_Click(sender, EventArgs.Empty); e.Handled = true; break;
                case Keys.F6: DeleteSelectedProfile(sender, EventArgs.Empty); e.Handled = true; break;
                case Keys.F7: SearchButton_Click(sender, EventArgs.Empty); e.Handled = true; break;
                case Keys.F8: ActivateSelectedProfile(sender, EventArgs.Empty); e.Handled = true; break;
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            OpenEditor(null);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            var profile = GetSelectedProfile();
            if (profile == null)
            {
                SetStatus("Selecione um perfil para editar.", true);
                return;
            }

            OpenEditor(profile);
        }

        private void OpenEditor(DatabaseProfile profile)
        {
            using (var editor = new EditorPerfilBancoDadosForm(_compositionRoot, _configuration, profile))
            {
                var result = editor.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    _hasChanges = true;
                    LoadConfiguration();
                    if (!string.IsNullOrEmpty(editor.SavedProfileId))
                    {
                        SelectProfileById(editor.SavedProfileId);
                    }
                    SetStatus("Perfil salvo com sucesso.", false);
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            using (var form = new DatabaseServerBrowserForm(_compositionRoot, _configuration))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _hasChanges = true;
                    LoadConfiguration();
                    SetStatus(form.ResultMessage ?? "Bancos adicionados com sucesso.", false);
                }
            }
        }

        private void CreateDatabaseButton_Click(object sender, EventArgs e)
        {
            using (var form = new DatabaseServerCreateForm(_compositionRoot, _configuration))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _hasChanges = true;
                    LoadConfiguration();
                    SetStatus(form.ResultMessage ?? "Banco criado com sucesso.", false);
                }
            }
        }

        private void DropDatabaseButton_Click(object sender, EventArgs e)
        {
            using (var form = new DatabaseServerDropForm(_compositionRoot, _configuration))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _hasChanges = true;
                    LoadConfiguration();
                    SetStatus(form.ResultMessage ?? "Banco excluido com sucesso.", false);
                }
            }
        }

        private void ManualButton_Click(object sender, EventArgs e)
        {
            if (_manualForm == null || _manualForm.IsDisposed)
            {
                _manualForm = new DatabaseManualForm();
                _manualForm.FormClosed += (_, __) => _manualForm = null;
            }

            _manualForm.Show(this);
            _manualForm.BringToFront();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void LoadConfiguration()
        {
            _configuration = _configurationController.LoadConfiguration();
            RefreshProfileList();
        }

        private void RefreshProfileList()
        {
            var profiles = _configuration.GetOrderedProfiles().ToArray();
            var activeId = _configuration.ActiveDatabaseId;

            _profilesListView.BeginUpdate();
            try
            {
                _profilesListView.Items.Clear();
                foreach (var profile in profiles)
                {
                    var isActive = !string.IsNullOrEmpty(activeId)
                        && string.Equals(profile.Id, activeId, StringComparison.OrdinalIgnoreCase);

                    var item = new ListViewItem(new[]
                    {
                        profile.Id ?? string.Empty,
                        profile.Name ?? string.Empty,
                        profile.Description ?? string.Empty,
                        (profile.Kind ?? string.Empty).ToUpperInvariant(),
                        profile.Host ?? string.Empty,
                        profile.Database ?? string.Empty,
                        isActive ? "ATIVO" : "INATIVO",
                    })
                    {
                        Tag = profile,
                        UseItemStyleForSubItems = true,
                        BackColor = isActive ? ActiveRowColor : Color.White,
                    };

                    _profilesListView.Items.Add(item);
                }

                if (_profilesListView.Items.Count > 0)
                {
                    SelectProfileById(activeId) ;
                }
            }
            finally
            {
                _profilesListView.EndUpdate();
            }
        }

        private bool SelectProfileById(string profileId)
        {
            if (_profilesListView.Items.Count == 0) return false;

            foreach (ListViewItem item in _profilesListView.Items)
            {
                if (item.Tag is DatabaseProfile candidate
                    && !string.IsNullOrEmpty(profileId)
                    && string.Equals(candidate.Id, profileId, StringComparison.OrdinalIgnoreCase))
                {
                    item.Selected = true;
                    item.EnsureVisible();
                    return true;
                }
            }

            _profilesListView.Items[0].Selected = true;
            _profilesListView.Items[0].EnsureVisible();
            return false;
        }

        private DatabaseProfile GetSelectedProfile()
        {
            if (_profilesListView.SelectedItems.Count == 0) return null;
            return _profilesListView.SelectedItems[0].Tag as DatabaseProfile;
        }

        private void DeleteSelectedProfile(object sender, EventArgs e)
        {
            var profile = GetSelectedProfile();
            if (profile == null)
            {
                SetStatus("Selecione um perfil para excluir.", true);
                return;
            }

            if (MessageBox.Show(this, $"Excluir o perfil '{profile.DisplayName}'?", "Confirmar Exclusao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            _configuration.DatabaseProfiles.Remove(profile.Id);
            if (string.Equals(_configuration.ActiveDatabaseId, profile.Id, StringComparison.OrdinalIgnoreCase))
            {
                _configuration.ActiveDatabaseId = _configuration.DatabaseProfiles.Keys.FirstOrDefault();
            }

            _configuration.IsConfigured = _configuration.DatabaseProfiles.Count > 0;
            _configurationController.SaveConfiguration(_configuration);
            _hasChanges = true;
            LoadConfiguration();
            SetStatus("Perfil removido.", false);
        }

        private void ActivateSelectedProfile(object sender, EventArgs e)
        {
            var profile = GetSelectedProfile();
            if (profile == null)
            {
                SetStatus("Selecione um perfil para ativar.", true);
                return;
            }

            _configuration.ActiveDatabaseId = profile.Id;
            _configurationController.SaveConfiguration(_configuration);
            _hasChanges = true;
            RefreshProfileList();
            SetStatus("Perfil ativo atualizado.", false);
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }
    }
}
