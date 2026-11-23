# Observabilidade

A aplicação mantém sinais básicos de operação sem registrar credenciais, tokens ou conteúdo de notificações.

## Correlation ID

Toda requisição que passa pelo pipeline HTTP recebe o cabeçalho `X-Correlation-ID` na resposta. Um valor recebido do cliente só é reutilizado quando contém no máximo 64 caracteres alfanuméricos ou `-`, `_` e `.`; caso contrário, a aplicação gera um identificador aleatório.

O middleware registra um evento estruturado ao final da requisição com:

- método HTTP;
- caminho sem query string;
- status HTTP;
- duração em milissegundos;
- `CorrelationId` no escopo do log.

O console usa o formatter JSON em ambientes baseados no `appsettings.json`, facilitando a ingestão por agregadores. O correlation ID pode ser usado para relacionar uma resposta ao log correspondente.

## Health checks

- `GET /health/live`: liveness, não consulta o banco e indica que o processo está respondendo;
- `GET /health/ready`: readiness, verifica conexão com o PostgreSQL;
- `GET /health`: alias de compatibilidade para readiness.

As respostas expõem apenas `{"status":"ok"}` ou `{"status":"unhealthy"}`. Detalhes da exceção do banco não são enviados ao cliente.

## Limitações atuais

- Os logs são escritos no console da instância; a retenção, agregação e alertas dependem do ambiente de execução.
- Ainda não há métricas Prometheus, tracing distribuído ou alertas gerenciados.
- O rate limiting é particionado pelo IP e permanece local a cada instância. Um ambiente com múltiplas réplicas precisa de um limitador distribuído antes de considerar o controle global.
