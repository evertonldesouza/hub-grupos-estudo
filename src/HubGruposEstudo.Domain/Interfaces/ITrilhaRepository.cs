using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface ITrilhaRepository
{
    Task<List<Trilha>> ListarAsync(int pagina = 1, int tamanhoPagina = 20);
    Task<Trilha?> BuscarPorIdAsync(Guid id);
    Task<List<Trilha>> BuscarPorHabilidadeAsync(Guid habilidadeId, int pagina = 1, int tamanhoPagina = 20);
    Task AdicionarAsync(Trilha trilha);
}
