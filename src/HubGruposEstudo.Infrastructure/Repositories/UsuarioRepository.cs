using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ListarAsync(string? busca = null)
    {
        var query = _context.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(u => u.Nome.ToLower().Contains(termo));
        }

        return await query
            .OrderBy(u => u.Nome)
            .Take(50)
            .ToListAsync();
    }

    public async Task<bool> ExistePorEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLower();
        return await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == emailNormalizado);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLower();
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);
    }

    public async Task<Usuario?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Usuarios
            .Include(u => u.Habilidades)
            .ThenInclude(uh => uh.Habilidade)
            .Include(u => u.Conquistas)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
