using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IInscricaoRepository
{
    Task<Inscricao?> BuscarPorIdAsync(Guid id);
    Task<Inscricao?> BuscarPorUsuarioTrilhaAsync(Guid usuarioId, Guid trilhaId);
    Task<List<Inscricao>> ListarPorUsuarioAsync(Guid usuarioId);
    Task AdicionarAsync(Inscricao inscricao);
    Task AtualizarAsync(Inscricao inscricao);
}
