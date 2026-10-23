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
        if (id == Guid.Empty)
            return null;

        return await _repository.BuscarPorIdAsync(id);
    }

    public async Task<Habilidade> CriarAsync(string nome, string categoria)
    {
        nome = nome?.Trim() ?? string.Empty;
        categoria = categoria?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("Nome é obrigatório");

        if (nome.Length > 100)
            throw new InvalidOperationException("Nome pode ter no máximo 100 caracteres");

        if (string.IsNullOrWhiteSpace(categoria))
            throw new InvalidOperationException("Categoria é obrigatória");

        if (categoria.Length > 50)
            throw new InvalidOperationException("Categoria pode ter no máximo 50 caracteres");

        var existe = await _repository.ExistePorNomeAsync(nome);
        if (existe)
            throw new InvalidOperationException("Habilidade já cadastrada");

        var habilidade = new Habilidade(nome, categoria);
        await _repository.AdicionarAsync(habilidade);
        return habilidade;
    }
}
