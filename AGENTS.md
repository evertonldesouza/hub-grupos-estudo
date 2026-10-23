# HubGruposEstudo — Sessão opencode

## Skills
- dotnet-patterns
- csharp-testing
- api-design
- postgres-patterns
- database-migrations

## Stack
- .NET 10 (net10.0)
- Clean Architecture (Domain, Application, Infrastructure, Web)
- EF Core + PostgreSQL
- JWT (Bearer)
- Swagger (Swashbuckle)

## Commands
```bash
# subir banco + API
docker compose up --build

# ou rodar somente o banco e a API localmente
docker compose up -d postgres
dotnet run --project src/HubGruposEstudo.Web --launch-profile http

# testes
dotnet test tests/HubGruposEstudo.UnitTests
```

## Padrões
- Clean Architecture com separação clara de camadas
- Async/await com sufixo `Async`
- Autenticação JWT com Bearer token
- Validação via FluentValidation (se aplicável)
