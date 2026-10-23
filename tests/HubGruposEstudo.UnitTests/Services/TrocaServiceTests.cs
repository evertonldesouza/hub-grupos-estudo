using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class TrocaServiceTests
{
    private readonly Mock<ITrocaRepository> _trocaRepositoryMock = new();
    private readonly Mock<IHabilidadeRepository> _habilidadeRepositoryMock = new();
    private readonly Mock<ITrilhaRepository> _trilhaRepositoryMock = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();

    [Fact]
    public async Task CriarAsync_DeveCriarTroca_QuandoRecursosPertencemAosUsuarios()
    {
        var usuarioAId = Guid.NewGuid();
        var usuarioBId = Guid.NewGuid();
        var habilidadeAId = Guid.NewGuid();
        var habilidadeBId = Guid.NewGuid();
        var trilhaAId = Guid.NewGuid();
        var trilhaBId = Guid.NewGuid();
        var trilhaA = new Trilha("C#", "A", Domain.Enums.NivelDificuldade.Iniciante,
            usuarioAId, habilidadeAId);
        var trilhaB = new Trilha("Python", "B", Domain.Enums.NivelDificuldade.Iniciante,
            usuarioBId, habilidadeBId);

        ConfigurarUsuario(usuarioAId);
        ConfigurarUsuario(usuarioBId);
        _habilidadeRepositoryMock.Setup(r => r.BuscarPorIdAsync(habilidadeAId))
            .ReturnsAsync(new Habilidade("C#", "Backend"));
        _habilidadeRepositoryMock.Setup(r => r.BuscarPorIdAsync(habilidadeBId))
            .ReturnsAsync(new Habilidade("Python", "Backend"));
        _trilhaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trilhaAId)).ReturnsAsync(trilhaA);
        _trilhaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trilhaBId)).ReturnsAsync(trilhaB);

        var service = CriarService();
        var troca = await service.CriarAsync(usuarioAId, usuarioBId, habilidadeAId,
            habilidadeBId, trilhaAId, trilhaBId);

        Assert.Equal(usuarioAId, troca.UsuarioAId);
        Assert.Equal(usuarioBId, troca.UsuarioBId);
        Assert.Equal(Domain.Enums.StatusTroca.Pendente, troca.Status);
        _trocaRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Troca>()), Times.Once);
    }

    [Fact]
    public async Task AtivarAsync_DeveImpedirUsuarioQueNaoEParteDaTroca()
    {
        var trocaId = Guid.NewGuid();
        var usuarioAId = Guid.NewGuid();
        var usuarioBId = Guid.NewGuid();
        var outroUsuarioId = Guid.NewGuid();
        var troca = new Troca(usuarioAId, usuarioBId, Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        _trocaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trocaId)).ReturnsAsync(troca);

        var service = CriarService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.AtivarAsync(trocaId, outroUsuarioId));
        _trocaRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Troca>()), Times.Never);
    }

    [Fact]
    public async Task AtivarAsync_DevePermitirSomenteUsuarioB()
    {
        var trocaId = Guid.NewGuid();
        var usuarioAId = Guid.NewGuid();
        var usuarioBId = Guid.NewGuid();
        var troca = new Troca(usuarioAId, usuarioBId, Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        _trocaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trocaId)).ReturnsAsync(troca);

        var service = CriarService();

        await service.AtivarAsync(trocaId, usuarioBId);

        Assert.Equal(Domain.Enums.StatusTroca.Ativa, troca.Status);
        _trocaRepositoryMock.Verify(r => r.AtualizarAsync(troca), Times.Once);
    }

    private TrocaService CriarService()
    {
        return new TrocaService(
            _trocaRepositoryMock.Object,
            _habilidadeRepositoryMock.Object,
            _trilhaRepositoryMock.Object,
            _usuarioRepositoryMock.Object);
    }

    private void ConfigurarUsuario(Guid id)
    {
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync(new Usuario("Usuário", $"{id}@email.com", "hash"));
    }
}
