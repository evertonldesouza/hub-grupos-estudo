using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Domain.Interfaces;

public interface IHabilidadeRepository
{
    Task<List<Habilidade>> ListarAsync();
    Task<Habilidade?> BuscarPorIdAsync(Guid id);
    Task AdicionarAsync(Habilidade habilidade);
    Task<bool> ExistePorNomeAsync(string nome);
}
