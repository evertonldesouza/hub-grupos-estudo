using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class ConquistaRepository : IConquistaRepository
{
    private readonly AppDbContext _context;

    public ConquistaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Conquista>> ListarAsync()
    {
        return await _context.Conquistas
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Conquista?> BuscarPorCriterioAsync(string criterio)
    {
        return await _context.Conquistas
            .FirstOrDefaultAsync(c => c.Criterio == criterio);
    }

    public async Task AdicionarAsync(Conquista conquista)
    {
        await _context.Conquistas.AddAsync(conquista);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ConquistaUsuario>> ListarDoUsuarioAsync(Guid usuarioId)
    {
        return await _context.ConquistaUsuarios
            .AsNoTracking()
            .Include(c => c.Conquista)
            .Where(c => c.UsuarioId == usuarioId)
            .OrderByDescending(c => c.DataObtida)
            .ToListAsync();
    }

    public async Task<ConquistaUsuario?> BuscarDoUsuarioAsync(Guid usuarioId, Guid conquistaId)
    {
        return await _context.ConquistaUsuarios
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ConquistaId == conquistaId);
    }

    public async Task AdicionarConquistaUsuarioAsync(ConquistaUsuario conquistaUsuario)
    {
        await _context.ConquistaUsuarios.AddAsync(conquistaUsuario);
        await _context.SaveChangesAsync();
    }
}
