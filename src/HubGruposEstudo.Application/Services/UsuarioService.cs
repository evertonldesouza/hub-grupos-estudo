using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Usuario>> ListarAsync(string? busca = null)
    {
        return await _repository.ListarAsync(busca);
    }

    public async Task<Usuario?> BuscarPorIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _repository.BuscarPorIdAsync(id);
    }
}
