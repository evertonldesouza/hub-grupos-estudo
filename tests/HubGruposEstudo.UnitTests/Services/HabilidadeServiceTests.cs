using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class HabilidadeServiceTests
{
    private readonly Mock<IHabilidadeRepository> _repositoryMock;
    private readonly HabilidadeService _service;

    public HabilidadeServiceTests()
    {
        _repositoryMock = new Mock<IHabilidadeRepository>();
        _service = new HabilidadeService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarHabilidade_QuandoDadosValidos()
    {
        _repositoryMock.Setup(r => r.ExistePorNomeAsync("React")).ReturnsAsync(false);

        var habilidade = await _service.CriarAsync("React", "Frontend");

        Assert.NotNull(habilidade);
        Assert.Equal("React", habilidade.Nome);
        Assert.Equal("Frontend", habilidade.Categoria);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Habilidade>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoNomeJaExiste()
    {
        _repositoryMock.Setup(r => r.ExistePorNomeAsync("React")).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAsync("React", "Frontend"));

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Habilidade>()), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarLista()
    {
        var habilidades = new List<Habilidade>
        {
            new("React", "Frontend"),
            new("Python", "Backend")
        };
        _repositoryMock.Setup(r => r.ListarAsync()).ReturnsAsync(habilidades);

        var result = await _service.ListarAsync();

        Assert.Equal(2, result.Count);
    }
}
