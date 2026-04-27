using System;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class DatabaseManualForm : Form
    {
        public DatabaseManualForm()
        {
            InitializeComponent();
            _manualTextBox.Text = BuildManualText();
            _manualTextBox.SelectionStart = 0;
            _manualTextBox.SelectionLength = 0;
            _closeButton.Click += CloseButton_Click;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static string BuildManualText()
        {
            return
                "BRCSISTEM - MANUAL DE CONFIGURACAO\r\n" +
                "GUIA COMPLETO PARA OPERAR O GERENCIADOR DE BANCOS\r\n\r\n" +
                "CONCEITOS BASICOS\r\n" +
                "1. Esta tela gerencia configuracoes de acesso aos bancos PostgreSQL.\r\n" +
                "2. O grupo 'Gerenciar Configuracoes' registra como o sistema acessa cada banco.\r\n" +
                "3. O grupo 'Gerenciar Servidor PostgreSQL' cria e exclui bancos diretamente no servidor.\r\n\r\n" +
                "ATALHOS DE TECLADO\r\n" +
                "F2  - Adicionar manualmente\r\n" +
                "F3  - Editar selecionado\r\n" +
                "F4  - Fechar janela\r\n" +
                "F6  - Remover configuracao da lista\r\n" +
                "F7  - Buscar bancos no servidor\r\n" +
                "F8  - Ativar banco selecionado\r\n\r\n" +
                "OPERACOES PRINCIPAIS\r\n" +
                "Buscar e Adicionar\r\n" +
                "- Conecta ao servidor PostgreSQL.\r\n" +
                "- Lista os bancos disponiveis.\r\n" +
                "- Permite adicionar multiplos bancos a lista de configuracoes.\r\n\r\n" +
                "Adicionar Manual / Editar\r\n" +
                "- Registra ou ajusta host, porta, database, usuario e senha.\r\n" +
                "- Use 'Testar Conexao' antes de salvar sempre que possivel.\r\n\r\n" +
                "Remover da Lista\r\n" +
                "- Remove apenas a configuracao do BRCSISTEM.\r\n" +
                "- Nao apaga o banco do servidor PostgreSQL.\r\n\r\n" +
                "Ativar Banco\r\n" +
                "- Define qual banco o sistema vai usar.\r\n" +
                "- Depois de ativar, reinicie o BRCSISTEM para garantir que tudo sera carregado corretamente.\r\n\r\n" +
                "Criar Novo Banco\r\n" +
                "- Cria um banco vazio no servidor PostgreSQL.\r\n" +
                "- Requer credenciais com permissao de administrador.\r\n" +
                "- Opcionalmente adiciona a nova configuracao a lista automaticamente.\r\n\r\n" +
                "Excluir Banco\r\n" +
                "- Remove o banco do servidor e apaga todos os dados.\r\n" +
                "- A acao e irreversivel.\r\n" +
                "- Confira host, banco e backups antes de confirmar.\r\n\r\n" +
                "DICAS IMPORTANTES\r\n" +
                "- Use nomes descritivos para identificar producao, homologacao e testes.\r\n" +
                "- Nunca informe o host com http:// ou https://.\r\n" +
                "- Para nomes de banco, use apenas letras, numeros e underscore.\r\n" +
                "- Se estiver trabalhando em rede, valide host, porta e firewall.\r\n" +
                "- Mantenha as credenciais administrativas sob controle.\r\n\r\n" +
                "BOA PRATICA\r\n" +
                "- Faca backup antes de criar ou excluir bancos em ambiente real.\r\n" +
                "- Prefira testar primeiro em um banco de homologacao.\r\n" +
                "- Depois de alterar o banco ativo, feche e abra o sistema novamente.\r\n";
        }
    }
}
