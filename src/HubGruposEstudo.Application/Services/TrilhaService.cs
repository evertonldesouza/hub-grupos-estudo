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

    public async Task<List<Trilha>> ListarAsync(int pagina = 1, int tamanhoPagina = 20)
    {
        return await _trilhaRepository.ListarAsync(pagina, tamanhoPagina);
    }

    public async Task<List<Trilha>> BuscarPorHabilidadeAsync(
        Guid habilidadeId,
        int pagina = 1,
        int tamanhoPagina = 20)
    {
        if (habilidadeId == Guid.Empty)
            return new List<Trilha>();

        return await _trilhaRepository.BuscarPorHabilidadeAsync(
            habilidadeId, pagina, tamanhoPagina);
    }

    public async Task<Trilha?> BuscarPorIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _trilhaRepository.BuscarPorIdAsync(id);
    }

    public async Task<Trilha> CriarComEtapasAsync(Guid criadorId, Guid habilidadeId,
        string titulo, string descricao, NivelDificuldade nivel,
        List<(string Titulo, string Descricao, TipoEtapa Tipo, int Duracao)> etapas)
    {
        if (criadorId == Guid.Empty)
            throw new InvalidOperationException("Criador é obrigatório");

        if (habilidadeId == Guid.Empty)
            throw new InvalidOperationException("Habilidade é obrigatória");

        if (!Enum.IsDefined(nivel))
            throw new InvalidOperationException("Nível de dificuldade inválido");

        titulo = titulo?.Trim() ?? string.Empty;
        descricao = descricao?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(titulo))
            throw new InvalidOperationException("Título é obrigatório");

        if (titulo.Length > 200)
            throw new InvalidOperationException("Título pode ter no máximo 200 caracteres");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new InvalidOperationException("Descrição é obrigatória");

        if (descricao.Length > 2000)
            throw new InvalidOperationException("Descrição pode ter no máximo 2000 caracteres");

        if (etapas is null || etapas.Count == 0)
            throw new InvalidOperationException("Trilha precisa de pelo menos 1 etapa");

        if (etapas.Count > 100)
            throw new InvalidOperationException("Trilha pode ter no máximo 100 etapas");

        var criador = await _usuarioRepository.BuscarPorIdAsync(criadorId);
        if (criador is null)
            throw new InvalidOperationException("Criador não encontrado");

        var habilidade = await _habilidadeRepository.BuscarPorIdAsync(habilidadeId);
        if (habilidade is null)
            throw new InvalidOperationException("Habilidade não encontrada");

        var trilha = new Trilha(titulo, descricao, nivel, criadorId, habilidadeId);

        for (var i = 0; i < etapas.Count; i++)
        {
            var etapaInput = etapas[i];
            var etapaTitulo = etapaInput.Titulo?.Trim() ?? string.Empty;
            var etapaDescricao = etapaInput.Descricao?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(etapaTitulo))
                throw new InvalidOperationException($"Título da etapa {i + 1} é obrigatório");

            if (etapaTitulo.Length > 200)
                throw new InvalidOperationException($"Título da etapa {i + 1} pode ter no máximo 200 caracteres");

            if (string.IsNullOrWhiteSpace(etapaDescricao))
                throw new InvalidOperationException($"Descrição da etapa {i + 1} é obrigatória");

            if (etapaDescricao.Length > 2000)
                throw new InvalidOperationException($"Descrição da etapa {i + 1} pode ter no máximo 2000 caracteres");

            if (!Enum.IsDefined(etapaInput.Tipo))
                throw new InvalidOperationException($"Tipo da etapa {i + 1} é inválido");

            if (etapaInput.Duracao < 1 || etapaInput.Duracao > 600)
                throw new InvalidOperationException($"Duração da etapa {i + 1} deve estar entre 1 e 600 minutos");

            var etapa = new Etapa(trilha.Id, i + 1, etapaTitulo, etapaDescricao,
                etapaInput.Tipo, etapaInput.Duracao);
            trilha.AdicionarEtapa(etapa);
        }

        await _trilhaRepository.AdicionarAsync(trilha);
        return trilha;
    }
}
