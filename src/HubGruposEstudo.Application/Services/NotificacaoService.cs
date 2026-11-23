using HubGruposEstudo.Application.Interfaces;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _repository;

    public NotificacaoService(INotificacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacaoPagina> ListarAsync(
        Guid usuarioId,
        bool apenasNaoLidas = false,
        int pagina = 1,
        int tamanhoPagina = 20)
    {
        ValidarUsuario(usuarioId);
        pagina = Math.Max(1, pagina);
        tamanhoPagina = Math.Clamp(tamanhoPagina, 1, 50);

        var itens = await _repository.ListarAsync(
            usuarioId, apenasNaoLidas, pagina, tamanhoPagina);
        var totalNaoLidas = await _repository.ContarNaoLidasAsync(usuarioId);

        return new NotificacaoPagina(itens, pagina, tamanhoPagina, totalNaoLidas);
    }

    public async Task<int> ContarNaoLidasAsync(Guid usuarioId)
    {
        ValidarUsuario(usuarioId);
        return await _repository.ContarNaoLidasAsync(usuarioId);
    }

    public async Task<Notificacao> CriarAsync(
        Guid usuarioId,
        TipoNotificacao tipo,
        string titulo,
        string mensagem,
        Guid? recursoId = null,
        string? recursoTipo = null)
    {
        ValidarUsuario(usuarioId);
        var notificacao = new Notificacao(
            usuarioId, tipo, titulo, mensagem, recursoId, recursoTipo);

        await _repository.AdicionarAsync(notificacao);
        return notificacao;
    }

    public async Task MarcarComoLidaAsync(Guid usuarioId, Guid notificacaoId)
    {
        ValidarUsuario(usuarioId);
        if (notificacaoId == Guid.Empty)
            throw new InvalidOperationException("Notificação é obrigatória");

        var notificacao = await _repository
            .BuscarPorIdDoUsuarioAsync(notificacaoId, usuarioId);
        if (notificacao is null)
            throw new InvalidOperationException("Notificação não encontrada");

        notificacao.MarcarComoLida();
        await _repository.AtualizarAsync(notificacao);
    }

    public async Task MarcarTodasComoLidasAsync(Guid usuarioId)
    {
        ValidarUsuario(usuarioId);
        await _repository.MarcarTodasComoLidasAsync(usuarioId);
    }

    private static void ValidarUsuario(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("Usuário não identificado");
    }
}
