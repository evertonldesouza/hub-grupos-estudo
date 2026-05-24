using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface ITrocaRepository
{
    Task<List<Troca>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<Troca?> BuscarPorIdAsync(Guid id);
    Task AdicionarAsync(Troca troca);
    Task AtualizarAsync(Troca troca);
}
