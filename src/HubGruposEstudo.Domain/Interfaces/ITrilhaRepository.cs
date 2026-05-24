using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface ITrilhaRepository
{
    Task<List<Trilha>> ListarAsync();
    Task<Trilha?> BuscarPorIdAsync(Guid id);
    Task<List<Trilha>> BuscarPorHabilidadeAsync(Guid habilidadeId);
    Task AdicionarAsync(Trilha trilha);
}
