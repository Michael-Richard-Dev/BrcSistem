using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Domain.Catalog
{
    public static class LegacyModuleCatalog
    {
        public static ModuleDefinition[] Create()
        {
            return new[]
            {
                new ModuleDefinition("cadastro_fornecedor", "Cadastros", "Fornecedores", "cadastro_fornecedor", "views/cadastro_fornecedor.py", "Cadastro de fornecedores de materiais.", true),
                new ModuleDefinition("cadastro_embalagem", "Cadastros", "Embalagens", "cadastro_embalagem", "views/cadastro_embalagem.py", "Cadastro base de materiais e embalagens.", true),
                new ModuleDefinition("cadastro_produto", "Cadastros", "Produtos", "cadastro_produto", "views/cadastro_produto.py", "Cadastro de produtos finais.", true),
                new ModuleDefinition("cadastro_lote", "Cadastros", "Lotes", "cadastro_lote", "views/cadastro_lote.py", "Gestao de lotes e validade.", true),
                new ModuleDefinition("cadastro_almoxarifado", "Cadastros", "Almoxarifados", "cadastro_almoxarifado", "views/cadastro_almoxarifado.py", "Cadastro e manutencao dos almoxarifados.", true),
                new ModuleDefinition("movimentacao_entrada", "Movimentacoes", "Entrada de Produtos", "movimentacao_entrada", "views/movimentacao_entrada.py", "Lancamento de notas fiscais de entrada.", true),
                new ModuleDefinition("movimentacao_transferencia", "Movimentacoes", "Transferencia entre Almoxarifados", "movimentacao_transferencia", "views/movimentacao_transferencia.py", "Transferencias internas de estoque.", true),
                new ModuleDefinition("movimentacao_saida_producao", "Movimentacoes", "Saida de Producao", "movimentacao_saida_producao", "views/movimentacao_saida_producao.py", "Baixa de materiais para producao com FIFO.", true),
                new ModuleDefinition("movimentacao_requisicao", "Movimentacoes", "Requisicao de Materiais", "movimentacao_requisicao", "views/movimentacao_requisicao.py", "Reserva e atendimento de requisicoes.", true),
                new ModuleDefinition("movimentacao_inventario", "Inventario", "Inventario de Estoque", "movimentacao_inventario", "views/movimentacao_inventario.py", "Abertura, contagem e fechamento de inventarios.", true),
                new ModuleDefinition("conta_corrente_estoque", "Consultas e Relatorios", "Conta Corrente de Estoque", "conta_corrente_estoque", "views/conta_corrente_estoque.py", "Extrato analitico das movimentacoes de estoque.", true),
                new ModuleDefinition("resumo_sintetico", "Consultas e Relatorios", "Resumo Sintetico", "resumo_sintetico", "views/resumo_sintetico.py", "Resumo gerencial consolidado do estoque.", true),
                new ModuleDefinition("relatorio_movimentacao_estoque", "Consultas e Relatorios", "Movimentacao de Estoque", "relatorio_movimentacao_estoque", "views/relatorio_movimentacao_estoque.py", "Filtros e consultas detalhadas de movimentos.", true),
                new ModuleDefinition("relatorio_entrada_pdf", "Consultas e Relatorios", "Relatorio de Entrada (PDF)", "relatorio_entrada_pdf", "views/relatorio_entrada_pdf.py", "Geracao de PDF de entradas.", true),
                new ModuleDefinition("relatorio_producao_saida_pdf", "Consultas e Relatorios", "Relatorio de Producao - Saida (PDF)", "relatorio_entrada_pdf", "views/relatorio_producao_saida_pdf.py", "Geracao de PDF para saidas de producao.", false),
                new ModuleDefinition("relatorio_transferencia_pdf", "Consultas e Relatorios", "Relatorio de Transferencias (PDF)", "relatorio_transferencia_pdf", "views/relatorio_transferencia_pdf.py", "Geracao de PDF para transferencias.", false),
                new ModuleDefinition("relatorio_inventario_pdf", "Consultas e Relatorios", "Relatorio de Inventario (PDF)", "relatorio_inventario_pdf", "views/relatorio_inventario_pdf.py", "Geracao de PDF para inventarios.", false),
                new ModuleDefinition("alerta_estoque_negativo_antes_entrada", "Auditoria", "Estoque Negativo antes da Entrada", "alerta_estoque_negativo_antes_entrada", "views/alerta_estoque_negativo_antes_entrada.py", "Analise de inconsistencias historicas de saldo.", false),
                new ModuleDefinition("bd_inconsistencias_lote_material", "Auditoria", "Inconsistencias Lote x Material", "bd_inconsistencias_lote_material", "views/bd_inconsistencias_lote_material.py", "Detector de divergencias entre lotes e materiais.", false),
                new ModuleDefinition("alerta_lote_descricao_duplicada_material", "Auditoria", "Descricao de Lote Duplicada por Material", "alerta_lote_descricao_duplicada_material", "views/alerta_lote_descricao_duplicada_material.py", "Analise de lotes duplicados por descricao.", false),
                new ModuleDefinition("alerta_movimentos_duplicados_nota", "Auditoria", "NFs com Movimentos Duplicados", "alerta_movimentos_duplicados_nota", "views/alerta_movimentos_duplicados_nota.py", "Procura por movimentos repetidos em notas fiscais.", false),
                new ModuleDefinition("alerta_entrada_lote_divergente", "Auditoria", "Entradas com Lote Divergente", "alerta_entrada_lote_divergente", "views/alerta_entrada_lote_divergente.py", "Valida divergencias de lote na entrada.", false),
                new ModuleDefinition("bd_remover_nota", "Banco de Dados", "Remover Nota Fiscal", "bd_remover_nota", "views/bd_remover_nota.py", "Exclusao controlada de notas.", false),
                new ModuleDefinition("bd_remover_transferencia", "Banco de Dados", "Remover Transferencia", "bd_remover_transferencia", "views/bd_remover_transferencia.py", "Exclusao controlada de transferencias.", false),
                new ModuleDefinition("bd_remover_saida", "Banco de Dados", "Remover Saida", "bd_remover_saida", "views/bd_remover_saida.py", "Exclusao controlada de saidas.", false),
                new ModuleDefinition("bd_remover_requisicao", "Banco de Dados", "Remover Requisicao", "bd_remover_requisicao", "views/bd_remover_requisicao.py", "Exclusao controlada de requisicoes.", false),
                new ModuleDefinition("bd_reativar_nota_entrada", "Banco de Dados", "Reativar Nota de Entrada", "bd_reativar_nota_entrada", "views/reativar_nota_entrada.py", "Reativacao de notas canceladas.", false),
                new ModuleDefinition("bd_alterar_data_transferencia", "Banco de Dados", "Alterar Data de Transferencia", "bd_alterar_data_transferencia", "views/bd_alterar_data_transferencia.py", "Correcao pontual de data de transferencia.", false),
                new ModuleDefinition("bd_alterar_data_entrada", "Banco de Dados", "Alterar Data de Entrada", "bd_alterar_data_entrada", "views/bd_alterar_data_entrada.py", "Correcao pontual de data de entrada.", false),
                new ModuleDefinition("bd_alterar_data_saida_producao", "Banco de Dados", "Alterar Data de Saida de Producao", "bd_alterar_data_saida_producao", "views/bd_alterar_data_saida_producao.py", "Correcao pontual de data de saida de producao.", false),
                new ModuleDefinition("bd_consulta_logs", "Banco de Dados", "Consultar Logs e Auditoria", "bd_consulta_logs", "views/bd_consulta_logs.py", "Consulta aos logs de auditoria.", false),
                new ModuleDefinition("cadastro_usuario", "Parametros", "Cadastro de Usuarios", "cadastro_usuario", "views/cadastro_usuario.py", "Cadastro de usuarios do sistema.", true),
                new ModuleDefinition("tipo_usuario", "Parametros", "Tipos de Usuario (Permissoes)", "tipo_usuario", "views/tipo_usuario.py", "Configuracao de perfis e permissoes.", true),
                new ModuleDefinition("gerenciar_acessos", "Parametros", "Solicitacoes de Acesso", "", "views/gerenciar_acessos.py", "Tratamento das solicitacoes de acesso pendentes.", false),
                new ModuleDefinition("parametros", "Parametros", "Parametros do Sistema", "parametros", "views/parametros.py", "Configuracoes funcionais e datas de fechamento.", false),
                new ModuleDefinition("parametro_sincronizar_movimentos_estoque", "Parametros", "Sincronizar Movimentos x Estoque", "parametro_sincronizar_movimentos_estoque", "views/parametro_sincronizar_movimentos_estoque.py", "Rotina de conciliacao entre movimentos e saldo.", false),
            };
        }
    }
}
