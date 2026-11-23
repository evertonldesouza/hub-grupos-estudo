using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface INotificacaoRepository
{
    Task<List<Notificacao>> ListarAsync(
        Guid usuarioId,
        bool apenasNaoLidas,
        int pagina,
        int tamanhoPagina);

    Task<int> ContarNaoLidasAsync(Guid usuarioId);
    Task<Notificacao?> BuscarPorIdDoUsuarioAsync(Guid id, Guid usuarioId);
    Task AdicionarAsync(Notificacao notificacao);
    Task AtualizarAsync(Notificacao notificacao);
    Task MarcarTodasComoLidasAsync(Guid usuarioId);
}
