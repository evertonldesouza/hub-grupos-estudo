using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class HabilidadeService
{
    private readonly IHabilidadeRepository _repository;

    public HabilidadeService(IHabilidadeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Habilidade>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<Habilidade?> BuscarPorIdAsync(Guid id)
    {
        return await _repository.BuscarPorIdAsync(id);
    }

    public async Task<Habilidade> CriarAsync(string nome, string categoria)
    {
        var existe = await _repository.ExistePorNomeAsync(nome);
        if (existe)
            throw new InvalidOperationException("Habilidade já cadastrada");

        var habilidade = new Habilidade(nome, categoria);
        await _repository.AdicionarAsync(habilidade);
        return habilidade;
    }
}
