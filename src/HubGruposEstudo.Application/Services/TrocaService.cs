using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class TrocaService
{
    private readonly ITrocaRepository _trocaRepository;
    private readonly IHabilidadeRepository _habilidadeRepository;
    private readonly ITrilhaRepository _trilhaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TrocaService(ITrocaRepository trocaRepository,
                        IHabilidadeRepository habilidadeRepository,
                        ITrilhaRepository trilhaRepository,
                        IUsuarioRepository usuarioRepository)
    {
        _trocaRepository = trocaRepository;
        _habilidadeRepository = habilidadeRepository;
        _trilhaRepository = trilhaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<Troca>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        return await _trocaRepository.ListarPorUsuarioAsync(usuarioId);
    }

    public async Task<Troca> CriarAsync(Guid usuarioAId, Guid usuarioBId,
        Guid habilidadeAId, Guid habilidadeBId,
        Guid trilhaAId, Guid trilhaBId)
    {
        var usuarioB = await _usuarioRepository.BuscarPorIdAsync(usuarioBId);
        if (usuarioB is null)
            throw new InvalidOperationException("Usuário não encontrado");

        var habA = await _habilidadeRepository.BuscarPorIdAsync(habilidadeAId);
        if (habA is null)
            throw new InvalidOperationException("Habilidade A não encontrada");

        var habB = await _habilidadeRepository.BuscarPorIdAsync(habilidadeBId);
        if (habB is null)
            throw new InvalidOperationException("Habilidade B não encontrada");

        var troca = new Troca(usuarioAId, usuarioBId, habilidadeAId, habilidadeBId,
                              trilhaAId, trilhaBId);

        await _trocaRepository.AdicionarAsync(troca);
        return troca;
    }

    public async Task<Troca> AtivarAsync(Guid trocaId)
    {
        var troca = await _trocaRepository.BuscarPorIdAsync(trocaId);
        if (troca is null)
            throw new InvalidOperationException("Troca não encontrada");

        troca.Ativar();
        await _trocaRepository.AtualizarAsync(troca);
        return troca;
    }

    public async Task<Troca> ConcluirAsync(Guid trocaId)
    {
        var troca = await _trocaRepository.BuscarPorIdAsync(trocaId);
        if (troca is null)
            throw new InvalidOperationException("Troca não encontrada");

        troca.Concluir();
        await _trocaRepository.AtualizarAsync(troca);
        return troca;
    }
}
