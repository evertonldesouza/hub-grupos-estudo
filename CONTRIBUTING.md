# Contribuindo

Este documento define o fluxo mínimo para evoluir o projeto sem perder decisões, cobertura ou controles de segurança.

## Antes de começar

1. Leia o [README.md](README.md) e a documentação relacionada à área alterada.
2. Identifique se a mudança toca domínio, API, banco, autenticação, interface, testes ou pipeline.
3. Defina como a mudança será validada e se exige migration, configuração ou rollback.

## Definition of done

Uma alteração está pronta para revisão quando:

- [ ] o código segue a separação Domain/Application/Infrastructure/Web;
- [ ] regras de negócio têm testes unitários;
- [ ] endpoints e integrações relevantes têm testes de integração;
- [ ] validações e autorização foram revisadas;
- [ ] nenhum secret, token, senha ou dado pessoal foi incluído no código, fixture ou log;
- [ ] migrations, seed e compatibilidade de banco foram avaliados;
- [ ] `README.md` foi atualizado quando a funcionalidade, configuração ou fluxo mudou;
- [ ] `CHANGELOG.md` recebeu uma entrada em `Unreleased`;
- [ ] `SECURITY.md` foi atualizado quando a mudança toca segurança ou dados;
- [ ] `docs/TESTING.md` foi atualizado quando a estratégia de testes mudou;
- [ ] `docs/CI-CD.md` foi atualizado quando pipeline, release ou deploy mudou;
- [ ] o CI passa e a documentação explica limitações conhecidas.

## Processo de uma nova implementação

### 1. Código e domínio

Mantenha regras de negócio no Domain/Application, controllers finos e acesso a dados atrás de interfaces. Evite duplicar validação somente no frontend.

### 2. Testes

Adicione o teste junto com a mudança. Para um endpoint protegido, cubra autenticação ausente, autorização insuficiente, entrada inválida e sucesso. Para uma mudança de banco, valide migrations no PostgreSQL.

Consulte [docs/TESTING.md](docs/TESTING.md).

### 3. Segurança

Revise secrets, dados pessoais, permissões, rate limiting, exposição de erros e configuração de produção. Registre a decisão no pull request e atualize [SECURITY.md](SECURITY.md).

### 4. Documentação e changelog

Use `CHANGELOG.md` para registrar o que mudou e o README para explicar como usar. Documentos específicos devem registrar o padrão, não apenas a implementação de um dia.

### 5. CI/CD

Não adicione dependência de máquina local ao workflow. Se alterar build, testes, banco, artefatos, release ou deploy, atualize [docs/CI-CD.md](docs/CI-CD.md).

## Commits

Prefira mensagens curtas e orientadas ao impacto:

```text
feat: adicionar filtro de trilhas por habilidade
fix: rejeitar avanço após conclusão da inscrição
test: cobrir recusa de troca por usuário não autorizado
docs: registrar política de releases
chore: atualizar dependências do CI
```

Não inclua secrets, dados de usuários ou dumps de banco em commits.

## Pull request

Preencha o template, descreva o problema, a solução, os testes executados e os riscos. Se a alteração exigir configuração manual, inclua passos de migração e rollback.
