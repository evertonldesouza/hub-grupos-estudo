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
# rodar com banco local
docker-compose up -d
dotnet run --project src/HubGruposEstudo.Web

# testes
dotnet test tests/HubGruposEstudo.UnitTests
```

## Padrões
- Clean Architecture com separação clara de camadas
- Async/await com sufixo `Async`
- Autenticação JWT com Bearer token
- Validação via FluentValidation (se aplicável)
