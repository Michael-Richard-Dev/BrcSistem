using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;
using Npgsql;

namespace BRCSISTEM.Desktop.Interface
{
    internal static class SuporteServidorBancoDados
    {
        public static bool IsLocalHost(string host)
        {
            var normalized = (host ?? string.Empty).Trim().ToLowerInvariant();
            return normalized == "localhost" || normalized == "127.0.0.1" || normalized == "::1";
        }

        public static string BuildUniqueProfileId(AppConfiguration configuration, string profileName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var probe = new DatabaseProfile { Name = profileName };
            var baseId = configuration.EnsureProfileId(probe);
            var candidate = baseId;
            var suffix = 2;

            while (configuration.DatabaseProfiles.ContainsKey(candidate))
            {
                candidate = baseId + "_" + suffix;
                suffix++;
            }

            return candidate;
        }

        public static int ParsePort(string value)
        {
            if (!int.TryParse((value ?? string.Empty).Trim(), out var port) || port <= 0 || port > 65535)
            {
                throw new InvalidOperationException("Informe uma porta valida entre 1 e 65535.");
            }

            return port;
        }

        public static void ValidateHost(string host)
        {
            var normalized = (host ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidOperationException("Informe o host do servidor.");
            }

            var lower = normalized.ToLowerInvariant();
            if (lower.StartsWith("http://") || lower.StartsWith("https://"))
            {
                throw new InvalidOperationException("Informe apenas o host ou IP, sem http:// ou https://.");
            }
        }

        public static void ValidateDatabaseName(string databaseName)
        {
            var trimmed = (databaseName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new InvalidOperationException("Informe o nome do banco.");
            }

            if (trimmed.Contains(" "))
            {
                throw new InvalidOperationException("O nome do banco nao pode conter espacos.");
            }

            foreach (var ch in trimmed)
            {
                if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    throw new InvalidOperationException("Use apenas letras, numeros e underscore (_) no nome do banco.");
                }
            }
        }

        public static List<string> ListDatabases(string host, int port, string user, string password)
        {
            using (var connection = new NpgsqlConnection(BuildAdminConnectionString(host, port, user, password)))
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT datname " +
                    "FROM pg_database " +
                    "WHERE datistemplate = false AND datname <> 'postgres' " +
                    "ORDER BY datname";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var databases = new List<string>();
                    while (reader.Read())
                    {
                        databases.Add(reader.GetString(0));
                    }

                    return databases;
                }
            }
        }

        public static void CreateDatabase(string host, int port, string user, string password, string databaseName)
        {
            using (var connection = new NpgsqlConnection(BuildAdminConnectionString(host, port, user, password)))
            {
                connection.Open();
                using (var existsCommand = connection.CreateCommand())
                {
                    existsCommand.CommandText = "SELECT 1 FROM pg_database WHERE datname = @name";
                    existsCommand.Parameters.AddWithValue("@name", databaseName);
                    var exists = existsCommand.ExecuteScalar();
                    if (exists != null)
                    {
                        throw new InvalidOperationException("Ja existe um banco com esse nome no servidor.");
                    }
                }

                using (var createCommand = connection.CreateCommand())
                {
                    createCommand.CommandText =
                        "CREATE DATABASE " + new NpgsqlCommandBuilder().QuoteIdentifier(databaseName) + " WITH ENCODING = 'UTF8'";
                    createCommand.ExecuteNonQuery();
                }
            }
        }

        public static void DropDatabase(string host, int port, string user, string password, string databaseName)
        {
            using (var connection = new NpgsqlConnection(BuildAdminConnectionString(host, port, user, password)))
            {
                connection.Open();
                using (var terminateCommand = connection.CreateCommand())
                {
                    terminateCommand.CommandText =
                        "SELECT pg_terminate_backend(pid) " +
                        "FROM pg_stat_activity " +
                        "WHERE datname = @name AND pid <> pg_backend_pid()";
                    terminateCommand.Parameters.AddWithValue("@name", databaseName);
                    terminateCommand.ExecuteNonQuery();
                }

                using (var dropCommand = connection.CreateCommand())
                {
                    var quotedName = new NpgsqlCommandBuilder().QuoteIdentifier(databaseName);
                    dropCommand.CommandText = "DROP DATABASE IF EXISTS " + quotedName;
                    dropCommand.ExecuteNonQuery();
                }
            }
        }

        public static DialogResult ShowTypedConfirmation(IWin32Window owner, string title, string prompt, string expectedText)
        {
            using (var form = new Form())
            using (var rootLayout = new TableLayoutPanel())
            using (var messageLabel = new Label())
            using (var inputTextBox = new TextBox())
            using (var buttonsLayout = new TableLayoutPanel())
            using (var okButton = new Button())
            using (var cancelButton = new Button())
            {
                form.Text = title;
                form.ClientSize = new Size(440, 170);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowInTaskbar = false;

                rootLayout.ColumnCount = 1;
                rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                rootLayout.RowCount = 3;
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                rootLayout.RowStyles.Add(new RowStyle());
                rootLayout.RowStyles.Add(new RowStyle());
                rootLayout.Dock = DockStyle.Fill;
                rootLayout.Padding = new Padding(15);

                messageLabel.AutoSize = true;
                messageLabel.Dock = DockStyle.Fill;
                messageLabel.Text = prompt;

                inputTextBox.Dock = DockStyle.Top;

                buttonsLayout.ColumnCount = 3;
                buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                buttonsLayout.ColumnStyles.Add(new ColumnStyle());
                buttonsLayout.ColumnStyles.Add(new ColumnStyle());
                buttonsLayout.Dock = DockStyle.Fill;
                buttonsLayout.Margin = new Padding(0, 10, 0, 0);

                okButton.Text = "Confirmar";
                okButton.DialogResult = DialogResult.OK;
                okButton.AutoSize = true;
                okButton.Margin = new Padding(5, 0, 0, 0);

                cancelButton.Text = "Cancelar";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.AutoSize = true;

                buttonsLayout.Controls.Add(new Panel(), 0, 0);
                buttonsLayout.Controls.Add(cancelButton, 1, 0);
                buttonsLayout.Controls.Add(okButton, 2, 0);

                rootLayout.Controls.Add(messageLabel, 0, 0);
                rootLayout.Controls.Add(inputTextBox, 0, 1);
                rootLayout.Controls.Add(buttonsLayout, 0, 2);

                form.Controls.Add(rootLayout);
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                if (form.ShowDialog(owner) != DialogResult.OK)
                {
                    return DialogResult.Cancel;
                }

                return string.Equals(inputTextBox.Text, expectedText, StringComparison.Ordinal)
                    ? DialogResult.OK
                    : DialogResult.Abort;
            }
        }

        private static string BuildAdminConnectionString(string host, int port, string user, string password)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = "postgres",
                Username = user,
                Password = password,
                Timeout = 10,
                CommandTimeout = 30,
                Pooling = false,
            };

            return builder.ConnectionString;
        }
    }
}
