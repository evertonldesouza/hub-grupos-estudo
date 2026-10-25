using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class TrocaService
{
    private readonly ITrocaRepository _trocaRepository;
    private readonly IHabilidadeRepository _habilidadeRepository;
    private readonly ITrilhaRepository _trilhaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConquistaRepository _conquistaRepository;

    public TrocaService(
        ITrocaRepository trocaRepository,
        IHabilidadeRepository habilidadeRepository,
        ITrilhaRepository trilhaRepository,
        IUsuarioRepository usuarioRepository,
        IConquistaRepository conquistaRepository)
    {
        _trocaRepository = trocaRepository;
        _habilidadeRepository = habilidadeRepository;
        _trilhaRepository = trilhaRepository;
        _usuarioRepository = usuarioRepository;
        _conquistaRepository = conquistaRepository;
    }

    public async Task<List<Troca>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            return new List<Troca>();

        return await _trocaRepository.ListarPorUsuarioAsync(usuarioId);
    }

    public async Task<Troca> CriarAsync(Guid usuarioAId, Guid usuarioBId,
        Guid habilidadeAId, Guid habilidadeBId,
        Guid trilhaAId, Guid trilhaBId)
    {
        ValidarGuid(usuarioAId, "Usuário A");
        ValidarGuid(usuarioBId, "Usuário B");
        ValidarGuid(habilidadeAId, "Habilidade A");
        ValidarGuid(habilidadeBId, "Habilidade B");
        ValidarGuid(trilhaAId, "Trilha A");
        ValidarGuid(trilhaBId, "Trilha B");

        if (usuarioAId == usuarioBId)
            throw new InvalidOperationException("Não é possível criar uma troca consigo mesmo");

        var usuarioA = await _usuarioRepository.BuscarPorIdAsync(usuarioAId);
        if (usuarioA is null)
            throw new InvalidOperationException("Usuário A não encontrado");

        var usuarioB = await _usuarioRepository.BuscarPorIdAsync(usuarioBId);
        if (usuarioB is null)
            throw new InvalidOperationException("Usuário B não encontrado");

        var habilidadeA = await _habilidadeRepository.BuscarPorIdAsync(habilidadeAId);
        if (habilidadeA is null)
            throw new InvalidOperationException("Habilidade A não encontrada");

        var habilidadeB = await _habilidadeRepository.BuscarPorIdAsync(habilidadeBId);
        if (habilidadeB is null)
            throw new InvalidOperationException("Habilidade B não encontrada");

        var trilhaA = await _trilhaRepository.BuscarPorIdAsync(trilhaAId);
        if (trilhaA is null)
            throw new InvalidOperationException("Trilha A não encontrada");

        var trilhaB = await _trilhaRepository.BuscarPorIdAsync(trilhaBId);
        if (trilhaB is null)
            throw new InvalidOperationException("Trilha B não encontrada");

        if (trilhaA.CriadorId != usuarioAId)
            throw new UnauthorizedAccessException("A trilha A não pertence ao usuário A");

        if (trilhaB.CriadorId != usuarioBId)
            throw new InvalidOperationException("A trilha B deve pertencer ao usuário B");

        if (trilhaA.HabilidadeId != habilidadeAId)
            throw new InvalidOperationException("A habilidade A não corresponde à trilha A");

        if (trilhaB.HabilidadeId != habilidadeBId)
            throw new InvalidOperationException("A habilidade B não corresponde à trilha B");

        var troca = new Troca(usuarioAId, usuarioBId, habilidadeAId, habilidadeBId,
            trilhaAId, trilhaBId);

        await _trocaRepository.AdicionarAsync(troca);
        return troca;
    }

    public async Task<Troca> AtivarAsync(Guid trocaId, Guid usuarioId)
    {
        var troca = await ObterTrocaDoUsuarioAsync(trocaId, usuarioId);

        if (troca.UsuarioBId != usuarioId)
            throw new UnauthorizedAccessException("Apenas o usuário B pode aceitar a troca");

        troca.Ativar();
        await _trocaRepository.AtualizarAsync(troca);
        return troca;
    }

    public async Task<Troca> RecusarAsync(Guid trocaId, Guid usuarioId)
    {
        var troca = await ObterTrocaDoUsuarioAsync(trocaId, usuarioId);

        if (troca.UsuarioBId != usuarioId)
            throw new UnauthorizedAccessException("Apenas o usuário B pode recusar a troca");

        troca.Recusar();
        await _trocaRepository.AtualizarAsync(troca);
        return troca;
    }

    public async Task<Troca> CancelarAsync(Guid trocaId, Guid usuarioId)
    {
        var troca = await ObterTrocaDoUsuarioAsync(trocaId, usuarioId);
        troca.Cancelar();
        await _trocaRepository.AtualizarAsync(troca);
        return troca;
    }

    public async Task<Troca> ConcluirAsync(Guid trocaId, Guid usuarioId)
    {
        var troca = await ObterTrocaDoUsuarioAsync(trocaId, usuarioId);
        troca.Concluir();
        await _trocaRepository.AtualizarAsync(troca);

        await ConcederRecompensaAsync(troca.UsuarioAId);
        await ConcederRecompensaAsync(troca.UsuarioBId);
        return troca;
    }

    private async Task ConcederRecompensaAsync(Guid usuarioId)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(usuarioId);
        if (usuario is not null)
        {
            usuario.AdicionarXp(50);
            await _usuarioRepository.AtualizarAsync(usuario);
        }

        var conquista = await _conquistaRepository
            .BuscarPorCriterioAsync("PRIMEIRA_TROCA_CONCLUIDA");
        if (conquista is null)
            return;

        var jaConquistou = await _conquistaRepository
            .BuscarDoUsuarioAsync(usuarioId, conquista.Id);
        if (jaConquistou is null)
        {
            await _conquistaRepository.AdicionarConquistaUsuarioAsync(
                new ConquistaUsuario(usuarioId, conquista.Id));
        }
    }

    private async Task<Troca> ObterTrocaDoUsuarioAsync(Guid trocaId, Guid usuarioId)
    {
        ValidarGuid(trocaId, "Troca");
        ValidarGuid(usuarioId, "Usuário");

        var troca = await _trocaRepository.BuscarPorIdAsync(trocaId);
        if (troca is null)
            throw new InvalidOperationException("Troca não encontrada");

        if (troca.UsuarioAId != usuarioId && troca.UsuarioBId != usuarioId)
            throw new UnauthorizedAccessException("Troca não pertence ao usuário");

        return troca;
    }

    private static void ValidarGuid(Guid value, string nome)
    {
        if (value == Guid.Empty)
            throw new InvalidOperationException($"{nome} é obrigatório");
    }
}
