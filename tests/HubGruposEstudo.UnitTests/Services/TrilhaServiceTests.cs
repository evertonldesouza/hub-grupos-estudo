using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class TrilhaServiceTests
{
    private readonly Mock<ITrilhaRepository> _trilhaRepositoryMock;
    private readonly Mock<IHabilidadeRepository> _habilidadeRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Application.Services.TrilhaService _service;

    public TrilhaServiceTests()
    {
        _trilhaRepositoryMock = new Mock<ITrilhaRepository>();
        _habilidadeRepositoryMock = new Mock<IHabilidadeRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _service = new Application.Services.TrilhaService(
            _trilhaRepositoryMock.Object,
            _habilidadeRepositoryMock.Object,
            _usuarioRepositoryMock.Object);
    }

    [Fact]
    public async Task CriarComEtapasAsync_DeveCriarTrilha_QuandoDadosValidos()
    {
        var habilidadeId = Guid.NewGuid();
        var criadorId = Guid.NewGuid();
        var habilidade = new Habilidade("React", "Frontend");

        _habilidadeRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(habilidadeId))
            .ReturnsAsync(habilidade);

        var etapas = new List<(string, string, TipoEtapa, int)>
        {
            ("Introdução", "O que é React", TipoEtapa.Artigo, 30),
            ("Componentes", "Criando componentes", TipoEtapa.Video, 45)
        };

        var trilha = await _service.CriarComEtapasAsync(
            criadorId, habilidadeId, "React do Zero", "Aprenda React",
            NivelDificuldade.Iniciante, etapas);

        Assert.NotNull(trilha);
        Assert.Equal("React do Zero", trilha.Titulo);
        Assert.Equal(2, trilha.Etapas.Count);
        _trilhaRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Trilha>()), Times.Once);
    }

    [Fact]
    public async Task CriarComEtapasAsync_DeveLancarExcecao_QuandoHabilidadeNaoExiste()
    {
        var habilidadeId = Guid.NewGuid();
        _habilidadeRepositoryMock
            .Setup(r => r.BuscarPorIdAsync(habilidadeId))
            .ReturnsAsync((Habilidade?)null);

        var etapas = new List<(string, string, TipoEtapa, int)>
        {
            ("Etapa 1", "Desc", TipoEtapa.Artigo, 30)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarComEtapasAsync(Guid.NewGuid(), habilidadeId,
                "Title", "Desc", NivelDificuldade.Iniciante, etapas));
    }
}
