using System;
using System.Linq;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Domain.Catalog
{
    public static class LegacyPermissionCatalog
    {
        public static PermissionCategory[] Create()
        {
            return new[]
            {
                new PermissionCategory("Sistema", new[]
                {
                    new PermissionDefinition("tela_principal", "Tela Principal"),
                    new PermissionDefinition("tela_login", "Login"),
                }),
                new PermissionCategory("Cadastros", new[]
                {
                    new PermissionDefinition("cadastro_fornecedor", "Cadastro de Fornecedor"),
                    new PermissionDefinition("cadastro_embalagem", "Cadastro de Embalagem"),
                    new PermissionDefinition("cadastro_produto", "Cadastro de Produto"),
                    new PermissionDefinition("cadastro_lote", "Cadastro de Lote"),
                    new PermissionDefinition("cadastro_almoxarifado", "Cadastro de Almoxarifado"),
                }),
                new PermissionCategory("Movimentacoes", new[]
                {
                    new PermissionDefinition("movimentacao_entrada", "Entrada de Produtos"),
                    new PermissionDefinition("movimentacao_transferencia", "Transferencia entre Almoxarifados"),
                    new PermissionDefinition("movimentacao_saida_producao", "Saida de Producao"),
                    new PermissionDefinition("movimentacao_requisicao", "Requisicao de Materiais"),
                    new PermissionDefinition("movimentacao_inventario", "Inventario de Estoque"),
                    new PermissionDefinition("inventario_abrir", "Inventario - Abrir"),
                    new PermissionDefinition("inventario_contar", "Inventario - Contar"),
                    new PermissionDefinition("inventario_fechar", "Inventario - Fechar"),
                    new PermissionDefinition("inventario_cancelar", "Inventario - Cancelar"),
                }),
                new PermissionCategory("Consulta e Relatorios", new[]
                {
                    new PermissionDefinition("conta_corrente_estoque", "Conta Corrente de Estoque"),
                    new PermissionDefinition("resumo_sintetico", "Resumo Sintetico de Estoque"),
                    new PermissionDefinition("relatorio_entrada_pdf", "Relatorio de Entrada (PDF)"),
                    new PermissionDefinition("relatorio_producao_saida_pdf", "Relatorio de Producao - Saida (PDF)"),
                    new PermissionDefinition("relatorio_transferencia_pdf", "Relatorio de Transferencias (PDF)"),
                    new PermissionDefinition("relatorio_inventario_pdf", "Relatorio de Inventario (PDF)"),
                    new PermissionDefinition("relatorio_movimentacao_estoque", "Relatorio de Movimentacao de Estoque"),
                }),
                new PermissionCategory("Alertas", new[]
                {
                    new PermissionDefinition("alerta_estoque_negativo_antes_entrada", "Estoque Negativo antes da Entrada"),
                    new PermissionDefinition("bd_inconsistencias_lote_material", "Inconsistencias Lote x Material"),
                    new PermissionDefinition("alerta_lote_descricao_duplicada_material", "Descricao de Lote Duplicada por Material"),
                    new PermissionDefinition("alerta_movimentos_duplicados_nota", "NFs com Movimentos Duplicados"),
                    new PermissionDefinition("alerta_entrada_lote_divergente", "Entradas com Lote Divergente"),
                }),
                new PermissionCategory("Banco de Dados", new[]
                {
                    new PermissionDefinition("bd_remover_nota", "Remover Nota Fiscal"),
                    new PermissionDefinition("bd_remover_transferencia", "Remover Transferencia"),
                    new PermissionDefinition("bd_remover_saida", "Remover Saida"),
                    new PermissionDefinition("bd_remover_requisicao", "Remover Requisicao"),
                    new PermissionDefinition("bd_reativar_nota_entrada", "Reativar Nota de Entrada"),
                    new PermissionDefinition("bd_alterar_data_entrada", "Alterar Data de Entrada"),
                    new PermissionDefinition("bd_alterar_data_transferencia", "Alterar Data de Transferencia"),
                    new PermissionDefinition("bd_alterar_data_saida_producao", "Alterar Data de Saida de Producao"),
                    new PermissionDefinition("bd_consulta_logs", "Consultar Logs"),
                }),
                new PermissionCategory("Parametros", new[]
                {
                    new PermissionDefinition("cadastro_usuario", "Cadastro de Usuarios"),
                    new PermissionDefinition("tipo_usuario", "Tipos de Usuario (Permissoes)"),
                    new PermissionDefinition("gerenciar_acessos", "Gerenciar Solicitacoes de Acesso"),
                    new PermissionDefinition("solicitacoes_acesso", "Gerenciar Acesso"),
                    new PermissionDefinition("parametros", "Parametros do Sistema"),
                    new PermissionDefinition("parametro_sincronizar_movimentos_estoque", "Sincronizar Movimentos x Estoque"),
                    new PermissionDefinition("consultar_logs_bd", "Consultar Logs e Auditoria"),
                    new PermissionDefinition("logs_auditoria", "Logs de Auditoria"),
                    new PermissionDefinition("trocar_senha", "Trocar Senha"),
                }),
            };
        }

        public static string[] GetAllPermissionKeys()
        {
            return Create()
                .SelectMany(category => category.Permissions)
                .Select(permission => permission.Key)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
