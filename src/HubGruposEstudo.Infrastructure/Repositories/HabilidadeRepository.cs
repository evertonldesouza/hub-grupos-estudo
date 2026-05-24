using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class HabilidadeRepository : IHabilidadeRepository
{
    private readonly AppDbContext _context;

    public HabilidadeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Habilidade>> ListarAsync()
    {
        return await _context.Habilidades.OrderBy(h => h.Nome).ToListAsync();
    }

    public async Task<Habilidade?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Habilidades.FindAsync(id);
    }

    public async Task AdicionarAsync(Habilidade habilidade)
    {
        await _context.Habilidades.AddAsync(habilidade);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistePorNomeAsync(string nome)
    {
        return await _context.Habilidades.AnyAsync(h => h.Nome == nome);
    }
}
