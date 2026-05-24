using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class TrilhaRepository : ITrilhaRepository
{
    private readonly AppDbContext _context;

    public TrilhaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trilha>> ListarAsync()
    {
        return await _context.Trilhas
            .Include(t => t.Habilidade)
            .Include(t => t.Criador)
            .Include(t => t.Etapas.OrderBy(e => e.Ordem))
            .ToListAsync();
    }

    public async Task<Trilha?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Trilhas
            .Include(t => t.Habilidade)
            .Include(t => t.Criador)
            .Include(t => t.Etapas.OrderBy(e => e.Ordem))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Trilha>> BuscarPorHabilidadeAsync(Guid habilidadeId)
    {
        return await _context.Trilhas
            .Include(t => t.Habilidade)
            .Include(t => t.Criador)
            .Where(t => t.HabilidadeId == habilidadeId)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Trilha trilha)
    {
        await _context.Trilhas.AddAsync(trilha);
        await _context.SaveChangesAsync();
    }
}
