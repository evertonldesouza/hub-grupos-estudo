using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IConquistaRepository
{
    Task<List<Conquista>> ListarAsync();
    Task AdicionarAsync(Conquista conquista);
    Task<List<ConquistaUsuario>> ListarDoUsuarioAsync(Guid usuarioId);
    Task AdicionarConquistaUsuarioAsync(ConquistaUsuario conquistaUsuario);
}
