using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class TrilhaService
{
    private readonly ITrilhaRepository _trilhaRepository;
    private readonly IHabilidadeRepository _habilidadeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TrilhaService(ITrilhaRepository trilhaRepository,
                         IHabilidadeRepository habilidadeRepository,
                         IUsuarioRepository usuarioRepository)
    {
        _trilhaRepository = trilhaRepository;
        _habilidadeRepository = habilidadeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<Trilha>> ListarAsync()
    {
        return await _trilhaRepository.ListarAsync();
    }

    public async Task<List<Trilha>> BuscarPorHabilidadeAsync(Guid habilidadeId)
    {
        return await _trilhaRepository.BuscarPorHabilidadeAsync(habilidadeId);
    }

    public async Task<Trilha?> BuscarPorIdAsync(Guid id)
    {
        return await _trilhaRepository.BuscarPorIdAsync(id);
    }

    public async Task<Trilha> CriarComEtapasAsync(Guid criadorId, Guid habilidadeId,
        string titulo, string descricao, NivelDificuldade nivel, List<(string Titulo, string Descricao, TipoEtapa Tipo, int Duracao)> etapas)
    {
        var habilidade = await _habilidadeRepository.BuscarPorIdAsync(habilidadeId);
        if (habilidade is null)
            throw new InvalidOperationException("Habilidade não encontrada");

        var trilha = new Trilha(titulo, descricao, nivel, criadorId, habilidadeId);

        for (int i = 0; i < etapas.Count; i++)
        {
            var e = etapas[i];
            var etapa = new Etapa(trilha.Id, i + 1, e.Titulo, e.Descricao, e.Tipo, e.Duracao);
            trilha.AdicionarEtapa(etapa);
        }

        await _trilhaRepository.AdicionarAsync(trilha);
        return trilha;
    }
}
