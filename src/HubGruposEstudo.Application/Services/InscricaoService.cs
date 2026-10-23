using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class InscricaoService
{
    private readonly IInscricaoRepository _inscricaoRepository;
    private readonly ITrilhaRepository _trilhaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public InscricaoService(IInscricaoRepository inscricaoRepository,
                            ITrilhaRepository trilhaRepository,
                            IUsuarioRepository usuarioRepository)
    {
        _inscricaoRepository = inscricaoRepository;
        _trilhaRepository = trilhaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<Inscricao>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            return new List<Inscricao>();

        return await _inscricaoRepository.ListarPorUsuarioAsync(usuarioId);
    }

    public async Task<Inscricao> InscreverAsync(Guid usuarioId, Guid trilhaId)
    {
        if (usuarioId == Guid.Empty)
            throw new InvalidOperationException("Usuário é obrigatório");

        if (trilhaId == Guid.Empty)
            throw new InvalidOperationException("Trilha é obrigatória");

        var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioId);
        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado");

        var trilha = await _trilhaRepository.BuscarPorIdAsync(trilhaId);
        if (trilha is null)
            throw new InvalidOperationException("Trilha não encontrada");

        if (trilha.Etapas.Count == 0)
            throw new InvalidOperationException("A trilha não possui etapas válidas");

        var existente = await _inscricaoRepository.BuscarPorUsuarioTrilhaAsync(usuarioId, trilhaId);
        if (existente is not null)
            throw new InvalidOperationException("Usuário já inscrito nesta trilha");

        var inscricao = new Inscricao(usuarioId, trilhaId);
        inscricao.AssociarTrilha(trilha);
        await _inscricaoRepository.AdicionarAsync(inscricao);
        return inscricao;
    }

    public async Task<Inscricao> AvancarEtapaAsync(Guid usuarioId, Guid inscricaoId)
    {
        var inscricao = await ObterInscricaoDoUsuarioAsync(usuarioId, inscricaoId);
        var totalEtapas = inscricao.Trilha?.Etapas.Count ?? 0;

        inscricao.AvancarEtapa(totalEtapas);
        await _inscricaoRepository.AtualizarAsync(inscricao);
        return inscricao;
    }

    public async Task<Inscricao> ConcluirAsync(Guid usuarioId, Guid inscricaoId)
    {
        var inscricao = await ObterInscricaoDoUsuarioAsync(usuarioId, inscricaoId);
        inscricao.Concluir();
        await _inscricaoRepository.AtualizarAsync(inscricao);
        return inscricao;
    }

    private async Task<Inscricao> ObterInscricaoDoUsuarioAsync(Guid usuarioId, Guid inscricaoId)
    {
        if (usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("Usuário não identificado");

        if (inscricaoId == Guid.Empty)
            throw new InvalidOperationException("Inscrição é obrigatória");

        var inscricao = await _inscricaoRepository.BuscarPorIdAsync(inscricaoId);
        if (inscricao is null)
            throw new InvalidOperationException("Inscrição não encontrada");

        if (inscricao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Inscrição não pertence ao usuário");

        return inscricao;
    }
}
