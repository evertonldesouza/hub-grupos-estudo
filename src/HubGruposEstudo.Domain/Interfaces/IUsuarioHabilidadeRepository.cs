using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IUsuarioHabilidadeRepository
{
    Task<List<UsuarioHabilidade>> ListarDoUsuarioAsync(Guid usuarioId);
    Task SubstituirAsync(Guid usuarioId, IReadOnlyCollection<Guid> habilidadeIds);
}
