using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class InscricaoServiceTests
{
    private readonly Mock<IInscricaoRepository> _inscricaoRepositoryMock;
    private readonly Mock<ITrilhaRepository> _trilhaRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IConquistaRepository> _conquistaRepositoryMock;
    private readonly Application.Services.InscricaoService _service;

    public InscricaoServiceTests()
    {
        _inscricaoRepositoryMock = new Mock<IInscricaoRepository>();
        _trilhaRepositoryMock = new Mock<ITrilhaRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _conquistaRepositoryMock = new Mock<IConquistaRepository>();
        _usuarioRepositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);
        _service = new Application.Services.InscricaoService(
            _inscricaoRepositoryMock.Object,
            _trilhaRepositoryMock.Object,
            _usuarioRepositoryMock.Object,
            _conquistaRepositoryMock.Object);
    }

    [Fact]
    public async Task InscreverAsync_DeveInscrever_QuandoTrilhaExiste()
    {
        var usuarioId = Guid.NewGuid();
        var trilhaId = Guid.NewGuid();
        var trilha = CriarTrilhaComEtapas();

        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(new Usuario("Everton", "everton@email.com", "hash"));
        _trilhaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trilhaId)).ReturnsAsync(trilha);
        _inscricaoRepositoryMock.Setup(r => r.BuscarPorUsuarioTrilhaAsync(usuarioId, trilhaId))
            .ReturnsAsync((Inscricao?)null);

        var inscricao = await _service.InscreverAsync(usuarioId, trilhaId);

        Assert.NotNull(inscricao);
        Assert.Equal(usuarioId, inscricao.UsuarioId);
        Assert.Equal(trilhaId, inscricao.TrilhaId);
        Assert.False(inscricao.Concluida);
        _inscricaoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Inscricao>()), Times.Once);
    }

    [Fact]
    public async Task InscreverAsync_DeveLancarExcecao_QuandoTrilhaNaoExiste()
    {
        var usuarioId = Guid.NewGuid();
        var trilhaId = Guid.NewGuid();
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(new Usuario("Everton", "everton@email.com", "hash"));
        _trilhaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trilhaId))
            .ReturnsAsync((Trilha?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.InscreverAsync(usuarioId, trilhaId));
    }

    [Fact]
    public async Task InscreverAsync_DeveLancarExcecao_QuandoJaInscrito()
    {
        var usuarioId = Guid.NewGuid();
        var trilhaId = Guid.NewGuid();
        var trilha = CriarTrilhaComEtapas();

        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(new Usuario("Everton", "everton@email.com", "hash"));
        _trilhaRepositoryMock.Setup(r => r.BuscarPorIdAsync(trilhaId)).ReturnsAsync(trilha);
        _inscricaoRepositoryMock.Setup(r => r.BuscarPorUsuarioTrilhaAsync(usuarioId, trilhaId))
            .ReturnsAsync(new Inscricao(usuarioId, trilhaId));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.InscreverAsync(usuarioId, trilhaId));
    }

    [Fact]
    public async Task AvancarEtapaAsync_DeveAvancar_QuandoInscricaoValida()
    {
        var usuarioId = Guid.NewGuid();
        var inscricaoId = Guid.NewGuid();
        var trilha = CriarTrilhaComEtapas();
        var inscricao = new Inscricao(usuarioId, trilha.Id);
        inscricao.AssociarTrilha(trilha);

        _inscricaoRepositoryMock.Setup(r => r.BuscarPorIdAsync(inscricaoId))
            .ReturnsAsync(inscricao);

        await _service.AvancarEtapaAsync(usuarioId, inscricaoId);

        Assert.Equal(2, inscricao.EtapaAtual);
        _inscricaoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Inscricao>()), Times.Once);
    }

    [Fact]
    public async Task AvancarEtapaAsync_DeveConcluirAoAvancarDaUltimaEtapa()
    {
        var usuarioId = Guid.NewGuid();
        var inscricaoId = Guid.NewGuid();
        var trilha = CriarTrilhaComEtapas();
        var inscricao = new Inscricao(usuarioId, trilha.Id);
        inscricao.AssociarTrilha(trilha);
        inscricao.AvancarEtapa(trilha.Etapas.Count);

        _inscricaoRepositoryMock.Setup(r => r.BuscarPorIdAsync(inscricaoId))
            .ReturnsAsync(inscricao);

        await _service.AvancarEtapaAsync(usuarioId, inscricaoId);

        Assert.True(inscricao.Concluida);
        _inscricaoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Inscricao>()), Times.Once);
    }

    private static Trilha CriarTrilhaComEtapas()
    {
        var trilha = new Trilha("React", "Desc", Domain.Enums.NivelDificuldade.Iniciante,
            Guid.NewGuid(), Guid.NewGuid());
        trilha.AdicionarEtapa(new Etapa(trilha.Id, 1, "Introdução", "Desc",
            Domain.Enums.TipoEtapa.Artigo, 30));
        trilha.AdicionarEtapa(new Etapa(trilha.Id, 2, "Componentes", "Desc",
            Domain.Enums.TipoEtapa.Exercicio, 45));
        return trilha;
    }
}
