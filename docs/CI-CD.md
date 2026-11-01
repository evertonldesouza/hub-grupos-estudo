# CI/CD e releases

## Estado atual

O repositório possui CI automatizado, mas ainda não possui deploy contínuo. O pipeline é a barreira de qualidade para pull requests e para a branch `main`; uma release deve ser criada somente depois de um CI verde.

Workflow: `.github/workflows/ci.yml`

## Gatilhos e permissões

O workflow executa em:

- push na `main`;
- pull request com destino à `main`.

O job usa `contents: read`, cancela execuções antigas da mesma referência quando uma nova alteração chega e possui timeout para não consumir runner indefinidamente.

## Etapas do CI

1. checkout do código;
2. setup do .NET 10;
3. inicialização do serviço PostgreSQL 16 com health check;
4. restore da solução;
5. build em `Release` sem novo restore;
6. testes unitários e de integração contra o PostgreSQL;
7. coleta e upload temporário da cobertura.

O CI deve permanecer determinístico: dependências, banco e variáveis usadas pelos testes precisam estar declarados no workflow ou na documentação. Secrets de produção nunca devem ser necessários para validar um pull request.

## Diagnóstico de falhas

Localmente, reproduza na mesma ordem do CI:

```bash
dotnet restore HubGruposEstudo.slnx
dotnet build HubGruposEstudo.slnx --configuration Release --no-restore
dotnet test tests/HubGruposEstudo.UnitTests --configuration Release --no-restore --verbosity normal
```

No GitHub CLI:

```bash
gh run list --branch main --limit 10
gh run watch <RUN_ID> --exit-status
gh run view <RUN_ID> --log-failed
```

Quando uma falha envolver banco, confira primeiro o health check do serviço PostgreSQL, a ordem das migrations e o isolamento entre testes.

## Estratégia de branch e pull request

- Faça mudanças pequenas e revisáveis.
- Todo pull request deve apontar para `main` e passar pelo CI.
- Não misture refatoração, mudança de schema e feature sem explicar o risco.
- Descreva migrations, alteração de configuração, impacto de segurança e plano de rollback.
- Use o checklist de `.github/PULL_REQUEST_TEMPLATE.md`.

## Releases

Use versionamento semântico:

- `MAJOR`: quebra de contrato ou comportamento incompatível;
- `MINOR`: funcionalidade compatível;
- `PATCH`: correção compatível.

Processo:

1. confirmar que o CI da `main` está verde;
2. atualizar `CHANGELOG.md`, movendo o conteúdo de `Unreleased` para a versão;
3. revisar README e documentação de segurança/testes/CI;
4. criar a tag anotada `vMAJOR.MINOR.PATCH` na `main`;
5. publicar uma GitHub Release com notas de migração e limitações;
6. validar health check e smoke tests do artefato publicado.

A release `v1.0.0` representa o MVP atual. O CI não publica imagem nem faz deploy automaticamente.

## Evolução planejada para CD

Antes de habilitar deploy automático, adicionar:

- ambientes separados para staging e produção;
- secrets por ambiente e menor privilégio possível;
- aprovação manual para produção;
- migrations como etapa controlada e reversível;
- health checks pós-deploy e rollback;
- retenção e assinatura dos artefatos;
- logs, métricas, tracing e alertas;
- imagem de container reproduzível e processo sem root.

Cada etapa de CD deve ser registrada neste documento e no `CHANGELOG.md`.
