# Notificações

As notificações são eventos persistidos destinados a um único usuário. A aplicação cria o registro depois de uma mudança de domínio relevante e a API sempre deriva o destinatário do JWT autenticado.

## Eventos registrados

| Tipo | Destinatário | Quando é criado |
| --- | --- | --- |
| `NovaTroca` | usuário B | uma nova proposta de troca é criada |
| `TrocaAceita` | usuário A | o usuário B aceita a proposta |
| `TrocaRecusada` | usuário A | o usuário B recusa a proposta |
| `TrocaCancelada` | outra pessoa da troca | uma troca pendente ou ativa é cancelada |
| `TrocaConcluida` | usuários A e B | uma troca ativa é concluída |
| `InscricaoCriada` | usuário inscrito | uma inscrição é realizada |
| `ProgressoAtualizado` | estudante | uma etapa é avançada sem concluir a trilha |
| `TrilhaConcluida` | estudante | a última etapa é avançada ou a trilha é concluída explicitamente |
| `ConquistaDesbloqueada` | estudante | uma conquista é concedida pela primeira vez |

Cada registro também guarda título, mensagem, data de criação, estado de leitura e, quando aplicável, o identificador e o tipo do recurso relacionado (`trilha`, `troca` ou `conquista`).

## API

Todos os endpoints abaixo exigem `Authorization: Bearer <token>`:

- `GET /api/notificacoes?apenasNaoLidas=false&pagina=1&tamanhoPagina=20` lista notificações do usuário autenticado. O tamanho é limitado a 50 e a resposta informa `totalNaoLidas`.
- `GET /api/notificacoes/nao-lidas/quantidade` retorna `{ "total": 0 }` para o usuário autenticado.
- `POST /api/notificacoes/{id}/ler` marca uma notificação pertencente ao usuário como lida. Uma notificação de outra pessoa não é revelada e resulta em `404`.
- `POST /api/notificacoes/ler-todas` marca todas as notificações pendentes do usuário como lidas.

O `UsuarioId` não é aceito como parâmetro nesses endpoints. Consultas, marcação individual e marcação em massa são filtradas no serviço e no repositório pelo identificador presente no token.

## Interface web

Depois do login, o sino do cabeçalho e o item **Notificações** da navegação mostram o total de não lidas. A página lista as atualizações mais recentes, permite marcar uma notificação individual e oferece a ação para marcar todas como lidas.

## Persistência

A tabela `notificacoes` é criada pela migration `20261023123000_AdicionarNotificacoes`. O índice `(UsuarioId, Lida, CriadaEm)` apoia a listagem e a contagem do badge. O relacionamento com `usuarios` usa exclusão em cascata, pois os registros pertencem ao usuário destinatário.
