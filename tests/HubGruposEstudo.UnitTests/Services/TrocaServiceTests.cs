using HubGruposEstudo.Application.Interfaces;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class TrocaServiceTests
{
    private readonly Mock<ITrocaRepository> _trocaRepositoryMock = new();
    private readonly Mock<IHabilidadeRepository> _habilidadeRepositoryMock = new();
    private readonly Mock<ITrilhaRepository> _trilhaRepositoryMock = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IConquistaRepository> _conquistaRepositoryMock = new();
    private readonly Mock<INotificacaoService> _notificacaoServiceMock = new();

    public TrocaServiceTests()
    {
        _notificacaoServiceMock
            .Setup(s => s.CriarAsync(
                It.IsAny<Guid>(),
                It.IsAny<TipoNotificacao>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<string?>()))
            .ReturnsAsync(new Notificacao(
                Guid.NewGuid(),
                TipoNotificacao.NovaTroca,
                "Teste",
                "Notificação de teste"));
    }

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
        _notificacaoServiceMock.Verify(s => s.CriarAsync(
            usuarioBId,
            TipoNotificacao.NovaTroca,
            "Nova proposta de troca",
            It.IsAny<string>(),
            troca.Id,
            "troca"), Times.Once);
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
        _notificacaoServiceMock.Verify(s => s.CriarAsync(
            usuarioAId,
            TipoNotificacao.TrocaAceita,
            "Troca aceita",
            It.IsAny<string>(),
            troca.Id,
            "troca"), Times.Once);
    }

    [Fact]
    public async Task RecusarAsync_DeveNotificarUsuarioQueCriouAProposta()
    {
        var usuarioAId = Guid.NewGuid();
        var usuarioBId = Guid.NewGuid();
        var troca = new Troca(usuarioAId, usuarioBId, Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        _trocaRepositoryMock.Setup(r => r.BuscarPorIdAsync(troca.Id)).ReturnsAsync(troca);

        var service = CriarService();

        await service.RecusarAsync(troca.Id, usuarioBId);

        Assert.Equal(StatusTroca.Recusada, troca.Status);
        _notificacaoServiceMock.Verify(s => s.CriarAsync(
            usuarioAId,
            TipoNotificacao.TrocaRecusada,
            "Troca recusada",
            It.IsAny<string>(),
            troca.Id,
            "troca"), Times.Once);
    }

    [Fact]
    public async Task ConcluirAsync_DeveNotificarAsDuasPessoasDaTroca()
    {
        var usuarioAId = Guid.NewGuid();
        var usuarioBId = Guid.NewGuid();
        var troca = new Troca(usuarioAId, usuarioBId, Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid());
        troca.Ativar();
        _trocaRepositoryMock.Setup(r => r.BuscarPorIdAsync(troca.Id)).ReturnsAsync(troca);

        var service = CriarService();

        await service.ConcluirAsync(troca.Id, usuarioAId);

        Assert.Equal(StatusTroca.Concluida, troca.Status);
        _notificacaoServiceMock.Verify(s => s.CriarAsync(
            usuarioAId,
            TipoNotificacao.TrocaConcluida,
            "Troca concluída",
            It.IsAny<string>(),
            troca.Id,
            "troca"), Times.Once);
        _notificacaoServiceMock.Verify(s => s.CriarAsync(
            usuarioBId,
            TipoNotificacao.TrocaConcluida,
            "Troca concluída",
            It.IsAny<string>(),
            troca.Id,
            "troca"), Times.Once);
    }

    private TrocaService CriarService()
    {
        return new TrocaService(
            _trocaRepositoryMock.Object,
            _habilidadeRepositoryMock.Object,
            _trilhaRepositoryMock.Object,
            _usuarioRepositoryMock.Object,
            _conquistaRepositoryMock.Object,
            _notificacaoServiceMock.Object);
    }

    private void ConfigurarUsuario(Guid id)
    {
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(id))
            .ReturnsAsync(new Usuario("Usuário", $"{id}@email.com", "hash"));
    }
}
