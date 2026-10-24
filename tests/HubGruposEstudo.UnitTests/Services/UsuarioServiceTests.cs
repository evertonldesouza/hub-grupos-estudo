using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IUsuarioHabilidadeRepository> _usuarioHabilidadeRepositoryMock = new();
    private readonly Mock<IHabilidadeRepository> _habilidadeRepositoryMock = new();

    [Fact]
    public async Task AtualizarPerfilAsync_DeveAtualizarNomeEBio()
    {
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario("Nome antigo", "usuario@email.com", "hash");
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId)).ReturnsAsync(usuario);
        _usuarioRepositoryMock.Setup(r => r.AtualizarAsync(usuario)).Returns(Task.CompletedTask);

        var service = CriarService();
        var atualizado = await service.AtualizarPerfilAsync(usuarioId, new AtualizarPerfilDto
        {
            Nome = "Nome novo",
            Bio = "Aprendendo todos os dias"
        });

        Assert.Equal("Nome novo", atualizado.Nome);
        Assert.Equal("Aprendendo todos os dias", atualizado.Bio);
        _usuarioRepositoryMock.Verify(r => r.AtualizarAsync(usuario), Times.Once);
    }

    [Fact]
    public async Task AtualizarHabilidadesAsync_DeveSubstituirRelacionamentos()
    {
        var usuarioId = Guid.NewGuid();
        var habilidade = new Habilidade("C#", "Backend");
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(new Usuario("Usuário", "usuario@email.com", "hash"));
        _habilidadeRepositoryMock.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Habilidade> { habilidade });
        _usuarioHabilidadeRepositoryMock
            .Setup(r => r.SubstituirAsync(usuarioId, It.IsAny<IReadOnlyCollection<Guid>>()))
            .Returns(Task.CompletedTask);

        var service = CriarService();
        var result = await service.AtualizarHabilidadesAsync(usuarioId, new[] { habilidade.Id });

        Assert.Single(result);
        Assert.Equal(habilidade.Id, result[0].Id);
        _usuarioHabilidadeRepositoryMock.Verify(
            r => r.SubstituirAsync(usuarioId, It.IsAny<IReadOnlyCollection<Guid>>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarHabilidadesAsync_DeveFalharQuandoHabilidadeNaoExiste()
    {
        var usuarioId = Guid.NewGuid();
        _usuarioRepositoryMock.Setup(r => r.BuscarPorIdAsync(usuarioId))
            .ReturnsAsync(new Usuario("Usuário", "usuario@email.com", "hash"));
        _habilidadeRepositoryMock.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Habilidade>());

        var service = CriarService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AtualizarHabilidadesAsync(usuarioId, new[] { Guid.NewGuid() }));
    }

    private UsuarioService CriarService()
    {
        return new UsuarioService(
            _usuarioRepositoryMock.Object,
            _usuarioHabilidadeRepositoryMock.Object,
            _habilidadeRepositoryMock.Object);
    }
}
