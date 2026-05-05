using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        _authService = new AuthService(_repositoryMock.Object);
    }

    [Fact]
    public async Task RegistrarAsync_DeveRegistrarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Everton Souza",
            Email = "everton@email.com",
            Senha = "123456"
        };

        _repositoryMock
            .Setup(r => r.ExistePorEmailAsync(dto.Email))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        // Act
        await _authService.RegistrarAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_DeveLancarExcecao_QuandoEmailJaCadastrado()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Everton Souza",
            Email = "everton@email.com",
            Senha = "123456"
        };

        _repositoryMock
            .Setup(r => r.ExistePorEmailAsync(dto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegistrarAsync(dto));

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
    }
}