# BRCSISTEM - Migracao Python para .NET Framework

## Estado atual

A solucao `BRCSISTEM.CSharp` foi criada para Visual Studio com quatro projetos:

- `BRCSISTEM.Domain`: modelos e catalogo de modulos.
- `BRCSISTEM.Application`: servicos, casos de uso e contratos.
- `BRCSISTEM.Infrastructure`: JSON de configuracao, sessao e acesso PostgreSQL.
- `BRCSISTEM.Desktop`: shell WinForms no modelo MVC.

## O que ja foi portado

- Leitura e escrita do `config_db.json` no mesmo formato usado pelo sistema Python.
- Bootstrap das tabelas minimas: `usuarios`, `tipos_usuario`, `parametros`, `logs_auditoria`, `registro_bloqueios` e `usuario_almoxarifados`.
- Autenticacao compatível com o Python:
  - hash `SHA256` com `GLOBAL_SALT`
  - fallback para `MD5` legado
  - troca obrigatoria de senha padrao
- Menu principal WinForms com os grupos funcionais do sistema Python.
- Mapeamento dos modulos principais para placeholders que apontam para o arquivo `.py` de origem.
- Persistencia simples dos ultimos modulos abertos em `session_state.json`.

## Validacao

Compilacao validada com sucesso via:

```powershell
dotnet build .\src\BRCSISTEM.Desktop\BRCSISTEM.Desktop.csproj `
  --configfile .\NuGet.Config `
  -p:RestoreIgnoreFailedSources=true `
  -p:BaseIntermediateOutputPath=C:\Users\micha\AppData\Local\Temp\BRCSISTEM.CSharp\obj\ `
  -p:BaseOutputPath=C:\Users\micha\AppData\Local\Temp\BRCSISTEM.CSharp\bin\ `
  -p:RestorePackagesPath=C:\Users\micha\AppData\Local\Temp\BRCSISTEM.CSharp\packages\
```

Observacao: foi necessario usar `AppData\Local\Temp` para os artefatos por causa do bloqueio de escrita do OneDrive nos diretórios de `obj/bin`.

## Publicacao

- Foi adicionada a solucao `BRCSISTEM.Desktop.sln` para abertura direta no Visual Studio.
- O `config_db.json` do projeto C# foi sanitizado antes da publicacao para evitar exposicao de credenciais reais.
- O provedor PostgreSQL continua desacoplado por reflexao; em tempo de execucao, o ambiente precisa ter `Npgsql` disponivel.

## Arquivos importantes

- `src/BRCSISTEM.Desktop/Program.cs`
- `src/BRCSISTEM.Desktop/Views/LoginForm.cs`
- `src/BRCSISTEM.Desktop/Views/MainForm.cs`
- `src/BRCSISTEM.Infrastructure/Configuration/JsonAppConfigurationStore.cs`
- `src/BRCSISTEM.Infrastructure/Database/PostgreSqlBootstrapper.cs`
- `src/BRCSISTEM.Application/Services/AuthenticationService.cs`
- `src/BRCSISTEM.Domain/Catalog/LegacyModuleCatalog.cs`

## Proxima fase recomendada

1. Portar os cadastros basicos:
   - `cadastro_fornecedor`
   - `cadastro_embalagem`
   - `cadastro_almoxarifado`
   - `cadastro_usuario`
2. Portar as movimentacoes centrais:
   - `movimentacao_entrada`
   - `movimentacao_transferencia`
   - `movimentacao_saida_producao`
   - `movimentacao_requisicao`
3. Migrar os relatorios e auditorias.
4. Substituir os placeholders do menu pelas telas reais.
