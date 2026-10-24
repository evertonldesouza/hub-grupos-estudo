# Hub de Grupos de Estudo

API para uma comunidade de aprendizagem em que pessoas criam trilhas, acompanham o próprio progresso e fazem trocas de conhecimento. O projeto está organizado em Clean Architecture:

- **Domain**: entidades, enums e regras de negócio;
- **Application**: casos de uso, serviços e DTOs;
- **Infrastructure**: PostgreSQL, Entity Framework Core, migrações e repositórios;
- **Web**: API REST, autenticação JWT e Swagger.

> O repositório entrega um MVP completo: a API REST e uma interface web leve servida pela própria aplicação. O frontend não usa um framework adicional, o que mantém o primeiro protótipo simples de executar e fácil de evoluir.

## Funcionalidades disponíveis

- cadastro e login com senha usando BCrypt;
- autenticação e autorização por JWT;
- catálogo inicial de habilidades;
- descoberta de usuários por nome sem expor email ou senha;
- perfil editável e habilidades que cada pessoa ensina;
- criação de trilhas com etapas ordenadas;
- consulta de trilhas por habilidade;
- inscrição, avanço de etapa e conclusão com percentual de progresso;
- XP, níveis e conquistas concedidos por eventos de estudo;
- criação, aceite, recusa, cancelamento e conclusão de trocas entre usuários;
- interface web responsiva servida em `/`;
- migração automática do banco e dados iniciais no primeiro start;
- endpoint `GET /health` para verificar a conexão com o PostgreSQL;
- Swagger com botão **Authorize** para testar endpoints protegidos.

## Rodando com Docker

Requisitos: Docker com Docker Compose.

```bash
cp .env.example .env
# edite .env e troque JWT_SECRET_KEY e ADMIN_PASSWORD
docker compose up --build
```

A aplicação ficará disponível em:

- Interface web: <http://localhost:8080>
- Swagger: <http://localhost:8080/swagger>
- Saúde: <http://localhost:8080/health>
- API: <http://localhost:8080/api>

O Compose aguarda o PostgreSQL ficar saudável, aplica as migrações e cria as habilidades iniciais. O administrador definido por `ADMIN_EMAIL` e `ADMIN_PASSWORD` também é criado somente se ainda não existir.

Para parar os containers:

```bash
docker compose down
```

Para apagar também os dados locais do PostgreSQL:

```bash
docker compose down -v
```

## Rodando localmente

Requisitos: .NET 10 SDK e PostgreSQL 16. Primeiro suba apenas o banco:

```bash
docker compose up -d postgres
```

Depois execute a API com a chave JWT configurada. O perfil de desenvolvimento já possui uma chave **somente para desenvolvimento local**:

```bash
dotnet run --project src/HubGruposEstudo.Web --launch-profile http
```

Nesse modo o banco local usa a conexão padrão de `appsettings.json`. Para criar um administrador local, informe as configurações antes de executar:

```bash
export BootstrapAdmin__Email=admin@local.test
export BootstrapAdmin__Password='uma-senha-local-forte'
dotnet run --project src/HubGruposEstudo.Web --launch-profile http
```

Ou use a composição completa, que já expõe a API na porta 8080.

## Fluxo rápido pelo Swagger

1. Use `POST /api/auth/registrar` para criar um membro.
2. Use `POST /api/auth/login` e copie o campo `token`.
3. Clique em **Authorize** e informe `Bearer <token>`.
4. Um administrador pode cadastrar novas habilidades em `POST /api/habilidades`. O catálogo inicial já é criado automaticamente.
5. Crie uma trilha em `POST /api/trilhas` com ao menos uma etapa.
6. Inscreva-se em `POST /api/inscricoes`, avance em `POST /api/inscricoes/{id}/avancar` e conclua em `POST /api/inscricoes/{id}/concluir`.

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

## Configuração

As configurações podem ser fornecidas por `appsettings*.json` ou por variáveis de ambiente usando `__` como separador:

| Configuração | Observação |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | conexão do PostgreSQL |
| `Jwt__SecretKey` | obrigatória fora do Development; mínimo de 32 caracteres |
| `Jwt__Issuer` / `Jwt__Audience` | valores esperados no token |
| `Jwt__ExpiracaoHoras` | duração do token, padrão 8 horas |
| `BootstrapAdmin__Email` | opcional; cria o administrador inicial |
| `BootstrapAdmin__Password` | opcional; mínimo de 6 caracteres |
| `Database__ApplyMigrations` | padrão `true`; desative apenas quando a migração for gerenciada externamente |

Não use os valores padrão do Compose em produção. Armazene a chave JWT, senha do banco e senha administrativa em um secret manager.

## Testes

```bash
dotnet restore HubGruposEstudo.slnx
dotnet test tests/HubGruposEstudo.UnitTests
dotnet build HubGruposEstudo.slnx
```

Os testes de serviço cobrem autenticação, habilidades, trilhas, inscrições, progresso, perfil e regras de acesso. Há também testes de integração para health check, cadastro e login usando PostgreSQL. O CI sobe um PostgreSQL temporário antes de executar a suíte.

## Próximos passos naturais

- notificações em tempo real ou por e-mail;
- criação de trocas diretamente pela interface, com seleção guiada de usuários e trilhas;
- testes de integração com PostgreSQL;
- paginação, observabilidade e limites de uso para produção.
