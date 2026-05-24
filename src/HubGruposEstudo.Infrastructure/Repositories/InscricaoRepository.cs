using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class InscricaoRepository : IInscricaoRepository
{
    private readonly AppDbContext _context;

    public InscricaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Inscricao?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Inscricoes
            .Include(i => i.Trilha)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Inscricao?> BuscarPorUsuarioTrilhaAsync(Guid usuarioId, Guid trilhaId)
    {
        return await _context.Inscricoes
            .FirstOrDefaultAsync(i => i.UsuarioId == usuarioId && i.TrilhaId == trilhaId);
    }

    public async Task<List<Inscricao>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Inscricoes
            .Include(i => i.Trilha)
            .ThenInclude(t => t.Habilidade)
            .Where(i => i.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Inscricao inscricao)
    {
        await _context.Inscricoes.AddAsync(inscricao);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Inscricao inscricao)
    {
        _context.Inscricoes.Update(inscricao);
        await _context.SaveChangesAsync();
    }
}
