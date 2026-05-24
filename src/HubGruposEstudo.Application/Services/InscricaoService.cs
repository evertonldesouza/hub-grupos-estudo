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
        return await _inscricaoRepository.ListarPorUsuarioAsync(usuarioId);
    }

    public async Task<Inscricao> InscreverAsync(Guid usuarioId, Guid trilhaId)
    {
        var trilha = await _trilhaRepository.BuscarPorIdAsync(trilhaId);
        if (trilha is null)
            throw new InvalidOperationException("Trilha não encontrada");

        var existente = await _inscricaoRepository.BuscarPorUsuarioTrilhaAsync(usuarioId, trilhaId);
        if (existente is not null)
            throw new InvalidOperationException("Usuário já inscrito nesta trilha");

        var inscricao = new Inscricao(usuarioId, trilhaId);
        await _inscricaoRepository.AdicionarAsync(inscricao);
        return inscricao;
    }

    public async Task<Inscricao> AvancarEtapaAsync(Guid usuarioId, Guid inscricaoId)
    {
        var inscricao = await _inscricaoRepository.BuscarPorIdAsync(inscricaoId);
        if (inscricao is null)
            throw new InvalidOperationException("Inscrição não encontrada");

        if (inscricao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Inscrição não pertence ao usuário");

        inscricao.AvancarEtapa();
        await _inscricaoRepository.AtualizarAsync(inscricao);
        return inscricao;
    }

    public async Task<Inscricao> ConcluirAsync(Guid usuarioId, Guid inscricaoId)
    {
        var inscricao = await _inscricaoRepository.BuscarPorIdAsync(inscricaoId);
        if (inscricao is null)
            throw new InvalidOperationException("Inscrição não encontrada");

        if (inscricao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Inscrição não pertence ao usuário");

        inscricao.Concluir();
        await _inscricaoRepository.AtualizarAsync(inscricao);
        return inscricao;
    }
}
