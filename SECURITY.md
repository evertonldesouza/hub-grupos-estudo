# Segurança

Este documento registra os controles atuais e o checklist mínimo para executar o Hub de Grupos de Estudo fora do ambiente local. Ele deve ser atualizado sempre que uma mudança tocar autenticação, autorização, dados pessoais, secrets, infraestrutura ou exposição de endpoints.

## Reporte responsável

Não publique detalhes de uma vulnerabilidade em issue pública. Envie um reporte privado aos mantenedores do repositório com:

- descrição e impacto;
- passos mínimos para reproduzir;
- versão, commit ou ambiente afetado;
- evidências sem incluir tokens, senhas ou dados pessoais;
- sugestão de mitigação, se houver.

Até existir um canal privado específico configurado no GitHub, o reporte deve ser feito por uma forma privada de contato com o proprietário do repositório. Não envie secrets reais em issues, pull requests ou logs de CI.

## Controles implementados

### Identidade e autorização

- Senhas são armazenadas com BCrypt; a senha original não é persistida.
- JWT exige chave com pelo menos 32 caracteres, issuer, audience, assinatura válida, expiração e validação de lifetime.
- Endpoints de escrita e dados pessoais usam `[Authorize]`.
- Cadastro e login são anônimos por necessidade funcional e recebem rate limiting.
- Criação de habilidades exige o papel de administrador.
- O perfil público não expõe email nem hash de senha.

### Proteção contra abuso

- A política `auth` permite no máximo 10 requisições por minuto por instância, sem fila, para endpoints de autenticação.
- O middleware de rate limiting é executado antes dos controllers.
- A limitação atual é local ao processo. Em múltiplas instâncias, deve ser substituída ou complementada por um mecanismo distribuído antes de considerar o controle suficiente para produção.

### Entrada, dados e erros

- DTOs usam validações de required, tamanho, formato e range quando aplicável.
- Regras de domínio são validadas nos serviços e entidades, não somente na interface.
- O acesso ao banco é feito pelo Entity Framework Core/Npgsql, evitando SQL concatenado nos casos de uso atuais.
- `ProblemDetails` e o exception handler evitam expor detalhes de exceção fora de Development.
- Swagger só é habilitado em Development.
- O endpoint `/health` retorna somente o estado de conectividade, sem credenciais ou detalhes do banco.

### Secrets e configuração

- `.env` é ignorado pelo Git; apenas `.env.example` é versionado.
- Chaves JWT, senhas do banco e senha de bootstrap devem vir de variáveis de ambiente ou de um secret manager.
- Os valores do Docker Compose são convenientes para desenvolvimento local e não são credenciais de produção.
- O ambiente de testes usa credenciais descartáveis do serviço PostgreSQL do CI; elas não devem ser reutilizadas em outros ambientes.

## Checklist antes de produção

- [ ] Substituir todos os valores padrão de banco, JWT e administrador.
- [ ] Armazenar secrets em um secret manager com rotação definida.
- [ ] Usar HTTPS no caminho completo da requisição e configurar `UseHttpsRedirection=true` quando aplicável.
- [ ] Configurar `AllowedHosts` para os hosts reais, em vez de aceitar `*` sem necessidade.
- [ ] Manter Swagger desabilitado fora de Development ou protegê-lo com uma política explícita.
- [ ] Restringir rede e credenciais do PostgreSQL; usar TLS quando o banco estiver fora da rede privada.
- [ ] Definir uma política de backup, restauração e retenção do PostgreSQL.
- [ ] Migrar rate limiting para armazenamento distribuído ao escalar horizontalmente.
- [ ] Executar migrations como etapa controlada de deploy quando houver mais de uma instância.
- [ ] Redigir tokens, senhas, connection strings e dados pessoais dos logs.
- [ ] Executar auditoria de dependências e corrigir vulnerabilidades antes da release.
- [ ] Executar testes de segurança e de autorização para cada novo endpoint protegido.
- [ ] Usar imagem de container atualizada e, idealmente, processo sem usuário root.

## Regra para novas funcionalidades

Uma mudança que introduza um endpoint, campo pessoal, fluxo de autenticação, papel, secret ou integração externa precisa atualizar este arquivo, incluir testes negativos e descrever no pull request:

1. qual dado é protegido;
2. quem pode acessar ou alterar;
3. como entradas inválidas são rejeitadas;
4. como o comportamento é observado e revertido;
5. quais secrets e permissões foram adicionados.
