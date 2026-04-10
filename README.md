# BRCSISTEM.CSharp

Migracao inicial do sistema Python BRCSISTEM para C# com .NET Framework 4.8, arquitetura em camadas e interface desktop WinForms no modelo MVC.

## Estrutura

- `BRCSISTEM.Domain`: modelos, seguranca e catalogo de modulos herdados do legado.
- `BRCSISTEM.Application`: casos de uso, servicos e contratos da aplicacao.
- `BRCSISTEM.Infrastructure`: leitura de configuracao JSON, sessao e integracao com PostgreSQL.
- `BRCSISTEM.Desktop`: shell desktop, controllers e telas WinForms.

## Solucoes

- `BRCSISTEM.Desktop.sln`
- `BRCSISTEM.Desktop.slnx`

## Estado da migracao

- Login e troca obrigatoria de senha baseados no legado Python.
- Persistencia do `config_db.json` no formato do sistema original.
- Bootstrap inicial das tabelas centrais do banco.
- Menu principal montado a partir dos modulos mapeados do sistema legado.
- Placeholders para os modulos ainda nao portados.

## Observacoes

- O arquivo `src/BRCSISTEM.Desktop/config/config_db.json` foi sanitizado para nao expor credenciais reais.
- A execucao com PostgreSQL depende da disponibilidade do provedor `Npgsql` em tempo de execucao.
- O resumo tecnico da conversao esta em `docs/MIGRACAO_PYTHON_PARA_DOTNET.md`.
