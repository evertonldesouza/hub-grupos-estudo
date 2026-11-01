# Hub de Grupos de Estudo

[![CI](https://github.com/evertonldesouza/hub-grupos-estudo/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/evertonldesouza/hub-grupos-estudo/actions/workflows/ci.yml)
[![Release](https://img.shields.io/github/v/release/evertonldesouza/hub-grupos-estudo)](https://github.com/evertonldesouza/hub-grupos-estudo/releases)

API e interface web para uma comunidade de aprendizagem em que pessoas criam trilhas, acompanham o próprio progresso e fazem trocas de conhecimento. O projeto está organizado em Clean Architecture e tem PostgreSQL como banco principal.

> A documentação faz parte da entrega. Toda mudança relevante deve atualizar o README, o changelog e, quando aplicável, os guias de segurança, testes e CI/CD. Consulte [CONTRIBUTING.md](CONTRIBUTING.md) antes de abrir uma alteração.

## Status do projeto

- Release estável atual: [v1.0.0](https://github.com/evertonldesouza/hub-grupos-estudo/releases/tag/v1.0.0).
- O pipeline de CI executa restore, build, testes automatizados e coleta de cobertura.
- O projeto ainda não possui deploy automático para um ambiente de produção; o CI é a barreira obrigatória antes de publicar uma release.

## Funcionalidades disponíveis

- cadastro e login com senha armazenada usando BCrypt;
- autenticação e autorização por JWT;
- rate limiting nos endpoints de autenticação;
- catálogo inicial de habilidades;
- descoberta de usuários por nome sem expor email ou senha;
- perfil editável e habilidades que cada pessoa ensina;
- criação de trilhas com etapas ordenadas;
- consulta paginada de trilhas por habilidade;
- inscrição, avanço de etapa e conclusão com percentual de progresso;
- XP, níveis e conquistas concedidos por eventos de estudo;
- criação, aceite, recusa, cancelamento e conclusão de trocas entre usuários;
- interface web responsiva servida em `/`;
- migrations do Entity Framework Core e dados iniciais no primeiro start;
- endpoint `GET /health` para verificar a conexão com o PostgreSQL;
- Swagger disponível somente no ambiente de desenvolvimento.

## Arquitetura

- **Domain**: entidades, enums e regras de negócio;
- **Application**: casos de uso, serviços e DTOs;
- **Infrastructure**: PostgreSQL, Entity Framework Core, migrations e repositórios;
- **Web**: API REST, autenticação JWT, rate limiting, Swagger e interface web estática;
- **Tests**: testes unitários de serviços e testes de integração HTTP/PostgreSQL.

## Documentação do projeto

| Documento | Conteúdo |
| --- | --- |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Definition of done, fluxo de desenvolvimento e regra de atualização da documentação |
| [SECURITY.md](SECURITY.md) | Controles de segurança atuais, checklist de produção e reporte de vulnerabilidades |
| [docs/TESTING.md](docs/TESTING.md) | Pirâmide de testes, execução local e como adicionar cenários |
| [docs/CI-CD.md](docs/CI-CD.md) | Pipeline, gates de qualidade, releases e evolução para CD |
| [CHANGELOG.md](CHANGELOG.md) | Histórico versionado das melhorias e mudanças relevantes |

## Requisitos

- .NET 10 SDK;
- PostgreSQL 16;
- Docker e Docker Compose para a execução recomendada;
- Git.

## Rodando com Docker

Copie o arquivo de exemplo e altere os valores antes de usar fora do ambiente local:

```bash
cp .env.example .env
docker compose up --build
```

A aplicação ficará disponível em:

- Interface web: <http://localhost:8080>
- API: <http://localhost:8080/api>
- Saúde: <http://localhost:8080/health>
- Swagger: disponível apenas quando `ASPNETCORE_ENVIRONMENT=Development`

O Compose aguarda o PostgreSQL ficar saudável, aplica as migrations e cria as habilidades e conquistas iniciais. O administrador definido por `ADMIN_EMAIL` e `ADMIN_PASSWORD` também é criado somente se ainda não existir.

Para parar os containers:

```bash
docker compose down
```

Para apagar também os dados locais do PostgreSQL:

```bash
docker compose down -v
```

> Os valores padrão do Compose servem apenas para desenvolvimento local. Em qualquer ambiente compartilhado, substitua banco, senha JWT e credenciais administrativas por secrets gerenciados.

## Rodando localmente

Suba apenas o banco:

```bash
docker compose up -d postgres
```

Depois execute a API com a configuração local:

```bash
dotnet run --project src/HubGruposEstudo.Web --launch-profile http
```

O perfil de desenvolvimento possui uma chave JWT de uso local. Nunca reutilize essa chave em staging ou produção.

Para criar um administrador local, informe as configurações antes de executar:

```bash
export BootstrapAdmin__Email=admin@local.test
export BootstrapAdmin__Password='uma-senha-local-forte'
dotnet run --project src/HubGruposEstudo.Web --launch-profile http
```

## Configuração

As configurações podem ser fornecidas por `appsettings*.json` ou por variáveis de ambiente usando `__` como separador:

| Configuração | Obrigatoriedade e observação |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | Obrigatória; conexão do PostgreSQL |
| `Jwt__SecretKey` | Obrigatória fora de Development; mínimo de 32 caracteres, aleatória e armazenada como secret |
| `Jwt__Issuer` / `Jwt__Audience` | Obrigatórias; devem ser os valores esperados pelos consumidores do token |
| `Jwt__ExpiracaoHoras` | Opcional; duração do token, padrão de 8 horas |
| `BootstrapAdmin__Email` | Opcional; cria o administrador inicial uma única vez |
| `BootstrapAdmin__Password` | Opcional; mínimo de 6 caracteres, nunca versionar |
| `Database__ApplyMigrations` | Opcional; padrão `true`, desative quando migrations forem gerenciadas externamente |
| `UseHttpsRedirection` | Recomendado `true` quando a aplicação receber tráfego diretamente |

Consulte o [checklist de segurança](SECURITY.md) antes de configurar um ambiente que não seja local.

## Fluxo rápido pelo Swagger

O Swagger é habilitado no ambiente de desenvolvimento. Para exercitar a API:

1. Use `POST /api/auth/registrar` para criar um membro.
2. Use `POST /api/auth/login` e copie o campo `token`.
3. Clique em **Authorize** e informe `Bearer <token>`.
4. Um administrador pode cadastrar novas habilidades em `POST /api/habilidades`.
5. Crie uma trilha em `POST /api/trilhas` com ao menos uma etapa.
6. Inscreva-se em `POST /api/inscricoes`, avance em `POST /api/inscricoes/{id}/avancar` e conclua em `POST /api/inscricoes/{id}/concluir`.
7. Use `/api/trocas` para propor, aceitar, recusar, cancelar ou concluir uma troca.

Exemplo de criação de trilha:

```json
{
  "trilha": {
    "titulo": "C# do zero",
    "descricao": "Fundamentos para começar a programar em C#.",
    "nivelDificuldade": "Iniciante",
    "habilidadeId": "COLOQUE-O-ID-DE-UMA-HABILIDADE"
  },
  "etapas": [
    {
      "titulo": "Variáveis e tipos",
      "descricao": "Conheça os tipos básicos da linguagem.",
      "tipo": "Artigo",
      "duracaoEstimadaMin": 30
    }
  ]
}
```

## Testes automatizados

O projeto usa xUnit, Moq, `WebApplicationFactory` e PostgreSQL real para integração:

```bash
dotnet restore HubGruposEstudo.slnx
dotnet build HubGruposEstudo.slnx --configuration Release --no-restore
dotnet test tests/HubGruposEstudo.UnitTests --configuration Release --no-restore --verbosity normal
```

Os testes de serviço cobrem autenticação, habilidades, trilhas, inscrições, progresso, perfil, XP, conquistas, trocas e regras de acesso. Os testes de integração cobrem health check, migrations, cadastro, login e perfil autenticado.

Para entender a organização e adicionar cenários corretamente, leia [docs/TESTING.md](docs/TESTING.md).

## Integração contínua e releases

O workflow em `.github/workflows/ci.yml` executa em pushes na `main` e em pull requests para `main`. Ele:

1. concede apenas a permissão `contents: read` ao job;
2. inicia um PostgreSQL 16 descartável com health check;
3. restaura dependências;
4. compila em Release;
5. executa testes unitários e de integração;
6. publica a cobertura como artefato temporário do workflow.

A publicação de uma versão deve seguir [docs/CI-CD.md](docs/CI-CD.md) e atualizar [CHANGELOG.md](CHANGELOG.md). O pipeline atual é CI; deploy contínuo deverá ser adicionado somente com ambientes, approvals, secrets e rollback definidos.

## Roadmap

- notificações em tempo real ou por e-mail;
- observabilidade com métricas, tracing e alertas;
- rate limiting distribuído para múltiplas instâncias;
- execução de migrations como etapa controlada de deploy;
- container sem usuário root e imagem com processo de atualização definido;
- ambiente de staging e CD com aprovação manual;
- testes de contrato e testes de carga para os endpoints críticos.

## Como documentar uma nova melhoria

Toda nova funcionalidade ou alteração técnica deve:

1. registrar a mudança em `CHANGELOG.md` na seção `Unreleased`;
2. atualizar a lista de funcionalidades e/ou configuração neste README;
3. atualizar `SECURITY.md` se tocar em autenticação, autorização, dados, secrets ou exposição de endpoint;
4. atualizar `docs/TESTING.md` e adicionar testes automatizados;
5. atualizar `docs/CI-CD.md` se alterar build, pipeline, release ou deploy;
6. registrar decisões relevantes no corpo do pull request;
7. marcar a documentação correspondente no checklist de [CONTRIBUTING.md](CONTRIBUTING.md).
