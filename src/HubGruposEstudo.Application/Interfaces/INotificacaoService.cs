using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Application.Interfaces;

public sealed record NotificacaoPagina(
    IReadOnlyList<Notificacao> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalNaoLidas);

public interface INotificacaoService
{
    Task<NotificacaoPagina> ListarAsync(
        Guid usuarioId,
        bool apenasNaoLidas = false,
        int pagina = 1,
        int tamanhoPagina = 20);

    Task<int> ContarNaoLidasAsync(Guid usuarioId);

    Task<Notificacao> CriarAsync(
        Guid usuarioId,
        TipoNotificacao tipo,
        string titulo,
        string mensagem,
        Guid? recursoId = null,
        string? recursoTipo = null);

    Task MarcarComoLidaAsync(Guid usuarioId, Guid notificacaoId);
    Task MarcarTodasComoLidasAsync(Guid usuarioId);
}
