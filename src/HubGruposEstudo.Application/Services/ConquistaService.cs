using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class ConquistaService
{
    private readonly IConquistaRepository _repository;

    public ConquistaService(IConquistaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Conquista>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<List<ConquistaUsuario>> ListarDoUsuarioAsync(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            return new List<ConquistaUsuario>();

        return await _repository.ListarDoUsuarioAsync(usuarioId);
    }
}
