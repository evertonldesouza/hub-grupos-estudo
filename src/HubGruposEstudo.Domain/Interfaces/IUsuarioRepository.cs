using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<bool> ExistePorEmailAsync(string email);
    Task AdicionarAsync(Usuario usuario);
    Task<Usuario?> BuscarPorEmailAsync(string email);
}