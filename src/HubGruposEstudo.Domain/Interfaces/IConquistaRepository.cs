using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IConquistaRepository
{
    Task<List<Conquista>> ListarAsync();
    Task<Conquista?> BuscarPorCriterioAsync(string criterio);
    Task AdicionarAsync(Conquista conquista);
    Task<List<ConquistaUsuario>> ListarDoUsuarioAsync(Guid usuarioId);
    Task<ConquistaUsuario?> BuscarDoUsuarioAsync(Guid usuarioId, Guid conquistaId);
    Task AdicionarConquistaUsuarioAsync(ConquistaUsuario conquistaUsuario);
}
