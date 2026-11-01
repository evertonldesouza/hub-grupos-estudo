# Testes automatizados

Este projeto usa uma combinação de testes unitários e testes de integração. O objetivo é manter feedback rápido para regras de negócio e, ao mesmo tempo, verificar o comportamento real da API, das migrations e do PostgreSQL.

## Pirâmide atual

### Testes unitários

Localizados em `tests/HubGruposEstudo.UnitTests/Services`:

- executam serviços isoladamente;
- usam Moq para repositórios e dependências;
- cobrem sucesso, autorização, validação e transições inválidas;
- não precisam de banco nem de servidor HTTP.

### Testes de integração

Localizados em `tests/HubGruposEstudo.UnitTests/Integration`:

- `ApiIntegrationTests` sobe a API com `WebApplicationFactory` e valida health, cadastro, login e perfil autenticado;
- `PostgresIntegrationTests` aplica migrations e verifica conexão e acesso às tabelas;
- o ambiente `Testing` usa PostgreSQL, não um banco falso, para detectar incompatibilidades de provider e schema;
- os testes são serializados porque compartilham o mesmo banco descartável.

## Pré-requisitos locais

Para executar a suíte completa, tenha .NET 10 SDK e PostgreSQL 16 disponíveis. A forma mais simples é:

```bash
docker compose up -d postgres
dotnet restore HubGruposEstudo.slnx
dotnet test tests/HubGruposEstudo.UnitTests --configuration Release
```

A fixture de integração espera, por padrão:

```text
Host=localhost;Port=5432;Database=hub_grupos_estudo;Username=hub_user;Password=hub_pass
```

Em CI, esses valores são fornecidos pelo serviço PostgreSQL do workflow. Nunca use credenciais reais nos testes.

## Comandos recomendados

```bash
# restaurar dependências
dotnet restore HubGruposEstudo.slnx

# compilar exatamente como o CI
dotnet build HubGruposEstudo.slnx --configuration Release --no-restore

# executar todos os testes
dotnet test tests/HubGruposEstudo.UnitTests --configuration Release --no-restore --verbosity normal

# executar somente uma classe ou teste
dotnet test tests/HubGruposEstudo.UnitTests --filter FullyQualifiedName~NomeDoTeste
```

Para subir apenas o banco ao terminar:

```bash
docker compose down
```

## Convenções para novos testes

- Nomeie o teste descrevendo comportamento, por exemplo `LoginAsync_DeveRecusarSenhaInvalida`.
- Use Arrange, Act e Assert, mantendo cada teste focado em uma regra.
- Cubra pelo menos o caminho feliz, uma entrada inválida e uma tentativa sem autorização quando o caso exigir.
- Não dependa da ordem entre testes; quando o banco for compartilhado, atualize a fixture e a estratégia de isolamento.
- Não use sleeps arbitrários para esconder race conditions; aguarde uma condição observável.
- Gere emails e identificadores únicos nos testes de integração.
- Não grave tokens, senhas ou connection strings em mensagens de falha.
- Ao alterar uma migration, mantenha um teste de aplicação de migrations no PostgreSQL.

## Cobertura

O CI coleta cobertura com `coverlet.collector` e publica o resultado como artefato temporário do workflow. A cobertura é um indicador para encontrar áreas sem testes; ela não substitui cenários de autorização, validação e integração.

A evolução planejada é adicionar um limite mínimo por camada, começando por Application e Domain, sem bloquear a equipe com uma porcentagem arbitrária antes de estabilizar a suíte.

## Definition of done para uma funcionalidade

Uma funcionalidade só está pronta quando:

- regras de negócio têm testes unitários;
- endpoints têm testes de integração quando atravessam autenticação, serialização ou banco;
- casos de erro e autorização estão cobertos;
- migrations e seed foram considerados, se houver alteração de modelo;
- o CI passa sem depender de configuração local;
- README, changelog e documentação específica foram atualizados.
