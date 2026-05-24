using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class TrocaRepository : ITrocaRepository
{
    private readonly AppDbContext _context;

    public TrocaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Troca>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Trocas
            .Include(t => t.UsuarioA)
            .Include(t => t.UsuarioB)
            .Include(t => t.HabilidadeA)
            .Include(t => t.HabilidadeB)
            .Where(t => t.UsuarioAId == usuarioId || t.UsuarioBId == usuarioId)
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync();
    }

    public async Task<Troca?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Trocas
            .Include(t => t.UsuarioA)
            .Include(t => t.UsuarioB)
            .Include(t => t.HabilidadeA)
            .Include(t => t.HabilidadeB)
            .Include(t => t.TrilhaA)
            .Include(t => t.TrilhaB)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AdicionarAsync(Troca troca)
    {
        await _context.Trocas.AddAsync(troca);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Troca troca)
    {
        _context.Trocas.Update(troca);
        await _context.SaveChangesAsync();
    }
}
