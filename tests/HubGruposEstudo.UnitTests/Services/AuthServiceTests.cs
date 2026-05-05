using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:SecretKey"])
            .Returns("chave-secreta-de-teste-super-segura-2026");
        _configurationMock.Setup(c => c["Jwt:Issuer"])
            .Returns("HubGruposEstudo");
        _configurationMock.Setup(c => c["Jwt:Audience"])
            .Returns("HubGruposEstudo");
        _configurationMock.Setup(c => c["Jwt:ExpiracaoHoras"])
            .Returns("8");

        _authService = new AuthService(_repositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegistrarAsync_DeveRegistrarUsuario_QuandoDadosValidos()
    {
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

        await _authService.RegistrarAsync(dto);

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_DeveLancarExcecao_QuandoEmailJaCadastrado()
    {
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Everton Souza",
            Email = "everton@email.com",
            Senha = "123456"
        };

        _repositoryMock
            .Setup(r => r.ExistePorEmailAsync(dto.Email))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegistrarAsync(dto));

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        var dto = new LoginDto
        {
            Email = "everton@email.com",
            Senha = "123456"
        };

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var usuario = new Usuario("Everton Souza", dto.Email, senhaHash);

        _repositoryMock
            .Setup(r => r.BuscarPorEmailAsync(dto.Email))
            .ReturnsAsync(usuario);

        var token = await _authService.LoginAsync(dto);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_DeveLancarExcecao_QuandoSenhaInvalida()
    {
        var dto = new LoginDto
        {
            Email = "everton@email.com",
            Senha = "senha-errada"
        };

        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha-correta");
        var usuario = new Usuario("Everton Souza", dto.Email, senhaHash);

        _repositoryMock
            .Setup(r => r.BuscarPorEmailAsync(dto.Email))
            .ReturnsAsync(usuario);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(dto));
    }
}