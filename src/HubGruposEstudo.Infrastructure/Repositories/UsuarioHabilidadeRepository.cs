using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class UsuarioHabilidadeRepository : IUsuarioHabilidadeRepository
{
    private readonly AppDbContext _context;

    public UsuarioHabilidadeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioHabilidade>> ListarDoUsuarioAsync(Guid usuarioId)
    {
        return await _context.UsuarioHabilidades
            .AsNoTracking()
            .Include(uh => uh.Habilidade)
            .Where(uh => uh.UsuarioId == usuarioId)
            .OrderBy(uh => uh.Habilidade.Nome)
            .ToListAsync();
    }

    public async Task SubstituirAsync(Guid usuarioId, IReadOnlyCollection<Guid> habilidadeIds)
    {
        var atuais = await _context.UsuarioHabilidades
            .Where(uh => uh.UsuarioId == usuarioId)
            .ToListAsync();

        _context.UsuarioHabilidades.RemoveRange(atuais);

        var novos = habilidadeIds
            .Distinct()
            .Select(habilidadeId => new UsuarioHabilidade(usuarioId, habilidadeId));

        await _context.UsuarioHabilidades.AddRangeAsync(novos);
        await _context.SaveChangesAsync();
    }
}
