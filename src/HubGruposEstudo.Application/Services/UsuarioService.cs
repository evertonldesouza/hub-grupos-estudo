using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IUsuarioHabilidadeRepository _usuarioHabilidadeRepository;
    private readonly IHabilidadeRepository _habilidadeRepository;

    public UsuarioService(
        IUsuarioRepository repository,
        IUsuarioHabilidadeRepository usuarioHabilidadeRepository,
        IHabilidadeRepository habilidadeRepository)
    {
        _repository = repository;
        _usuarioHabilidadeRepository = usuarioHabilidadeRepository;
        _habilidadeRepository = habilidadeRepository;
    }

    public async Task<List<Usuario>> ListarAsync(string? busca = null)
    {
        return await _repository.ListarAsync(busca);
    }

    public async Task<Usuario?> BuscarPorIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _repository.BuscarPorIdAsync(id);
    }

    public async Task<Usuario> AtualizarPerfilAsync(Guid usuarioId, AtualizarPerfilDto dto)
    {
        if (usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("Usuário não identificado");

        ArgumentNullException.ThrowIfNull(dto);

        var nome = dto.Nome?.Trim() ?? string.Empty;
        var bio = dto.Bio?.Trim();
        if (nome.Length < 2 || nome.Length > 100)
            throw new InvalidOperationException("Nome deve ter entre 2 e 100 caracteres");

        if (bio?.Length > 500)
            throw new InvalidOperationException("Bio pode ter no máximo 500 caracteres");

        var usuario = await _repository.BuscarPorIdAsync(usuarioId);
        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado");

        usuario.AtualizarNome(nome);
        usuario.AtualizarBio(bio);
        await _repository.AtualizarAsync(usuario);
        return usuario;
    }

    public async Task<List<Habilidade>> ListarHabilidadesAsync(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            return new List<Habilidade>();

        var relacionamentos = await _usuarioHabilidadeRepository.ListarDoUsuarioAsync(usuarioId);
        return relacionamentos
            .Where(relacionamento => relacionamento.Habilidade is not null)
            .Select(relacionamento => relacionamento.Habilidade)
            .ToList();
    }

    public async Task<List<Habilidade>> AtualizarHabilidadesAsync(
        Guid usuarioId,
        IReadOnlyCollection<Guid> habilidadeIds)
    {
        if (usuarioId == Guid.Empty)
            throw new UnauthorizedAccessException("Usuário não identificado");

        var usuario = await _repository.BuscarPorIdAsync(usuarioId);
        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado");

        var ids = habilidadeIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var habilidades = await _habilidadeRepository.ListarAsync();
        var existentes = habilidades
            .Where(habilidade => ids.Contains(habilidade.Id))
            .ToList();

        if (existentes.Count != ids.Count)
            throw new InvalidOperationException("Uma ou mais habilidades não foram encontradas");

        await _usuarioHabilidadeRepository.SubstituirAsync(usuarioId, ids);
        return existentes.OrderBy(habilidade => habilidade.Nome).ToList();
    }
}
