using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class NotificacaoRepository : INotificacaoRepository
{
    private readonly AppDbContext _context;

    public NotificacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notificacao>> ListarAsync(
        Guid usuarioId,
        bool apenasNaoLidas,
        int pagina,
        int tamanhoPagina)
    {
        var query = _context.Notificacoes
            .AsNoTracking()
            .Where(notificacao => notificacao.UsuarioId == usuarioId);

        if (apenasNaoLidas)
            query = query.Where(notificacao => !notificacao.Lida);

        return await query
            .OrderByDescending(notificacao => notificacao.CriadaEm)
            .ThenByDescending(notificacao => notificacao.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }

    public Task<int> ContarNaoLidasAsync(Guid usuarioId)
    {
        return _context.Notificacoes
            .CountAsync(notificacao =>
                notificacao.UsuarioId == usuarioId && !notificacao.Lida);
    }

    public Task<Notificacao?> BuscarPorIdDoUsuarioAsync(Guid id, Guid usuarioId)
    {
        return _context.Notificacoes
            .FirstOrDefaultAsync(notificacao =>
                notificacao.Id == id && notificacao.UsuarioId == usuarioId);
    }

    public async Task AdicionarAsync(Notificacao notificacao)
    {
        await _context.Notificacoes.AddAsync(notificacao);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Notificacao notificacao)
    {
        _context.Notificacoes.Update(notificacao);
        await _context.SaveChangesAsync();
    }

    public async Task MarcarTodasComoLidasAsync(Guid usuarioId)
    {
        var pendentes = await _context.Notificacoes
            .Where(notificacao => notificacao.UsuarioId == usuarioId && !notificacao.Lida)
            .ToListAsync();

        foreach (var notificacao in pendentes)
            notificacao.MarcarComoLida();

        if (pendentes.Count > 0)
            await _context.SaveChangesAsync();
    }
}
