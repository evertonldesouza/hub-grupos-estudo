## Resumo

<!-- Explique o problema e a solução em poucas linhas. -->

## Tipo de mudança

- [ ] Nova funcionalidade
- [ ] Correção
- [ ] Refatoração
- [ ] Migração/configuração
- [ ] Segurança
- [ ] Testes
- [ ] Documentação
- [ ] CI/CD

## Checklist de qualidade

- [ ] Testes unitários adicionados ou atualizados.
- [ ] Testes de integração adicionados quando a mudança atravessa HTTP, autenticação ou banco.
- [ ] Validação, autorização e tratamento de erros revisados.
- [ ] Nenhum secret, token, senha ou dado pessoal foi incluído.
- [ ] `README.md` atualizado quando uso/configuração/funcionalidade mudou.
- [ ] `CHANGELOG.md` atualizado em `Unreleased`.
- [ ] `SECURITY.md` atualizado se houver impacto de segurança ou dados.
- [ ] `docs/TESTING.md` atualizado se a estratégia de testes mudou.
- [ ] `docs/CI-CD.md` atualizado se o pipeline, release ou deploy mudou.
- [ ] Migrations, seed e rollback avaliados quando aplicável.
- [ ] CI executado com sucesso.

## Validação

<!-- Liste os comandos executados e resultados relevantes. -->

```text
# exemplo:
dotnet test tests/HubGruposEstudo.UnitTests --configuration Release
```

## Riscos e rollback

<!-- Explique riscos, mudanças de configuração, migrations e como reverter. -->
