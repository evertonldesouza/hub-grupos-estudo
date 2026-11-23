using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using HubGruposEstudo.Domain.Interfaces;
using Moq;

namespace HubGruposEstudo.UnitTests.Services;

public class NotificacaoServiceTests
{
    private readonly Mock<INotificacaoRepository> _repositoryMock = new();
    private readonly NotificacaoService _service;

    public NotificacaoServiceTests()
    {
        _service = new NotificacaoService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ListarAsync_DeveNormalizarPaginaETamanhoEInformarNaoLidas()
    {
        var usuarioId = Guid.NewGuid();
        _repositoryMock
            .Setup(repository => repository.ListarAsync(usuarioId, true, 1, 50))
            .ReturnsAsync(new List<Notificacao>());
        _repositoryMock
            .Setup(repository => repository.ContarNaoLidasAsync(usuarioId))
            .ReturnsAsync(3);

        var resultado = await _service.ListarAsync(
            usuarioId, apenasNaoLidas: true, pagina: 0, tamanhoPagina: 500);

        Assert.Equal(1, resultado.Pagina);
        Assert.Equal(50, resultado.TamanhoPagina);
        Assert.Equal(3, resultado.TotalNaoLidas);
        Assert.Empty(resultado.Itens);
    }

    [Fact]
    public async Task MarcarComoLidaAsync_DeveAtualizarSomenteNotificacaoDoUsuario()
    {
        var usuarioId = Guid.NewGuid();
        var notificacao = new Notificacao(
            usuarioId,
            TipoNotificacao.NovaTroca,
            "Nova troca",
            "Você recebeu uma proposta.");
        _repositoryMock
            .Setup(repository => repository.BuscarPorIdDoUsuarioAsync(notificacao.Id, usuarioId))
            .ReturnsAsync(notificacao);

        await _service.MarcarComoLidaAsync(usuarioId, notificacao.Id);

        Assert.True(notificacao.Lida);
        Assert.NotNull(notificacao.LidaEm);
        _repositoryMock.Verify(
            repository => repository.AtualizarAsync(notificacao),
            Times.Once);
    }

    [Fact]
    public async Task MarcarComoLidaAsync_DeveRecusarNotificacaoDeOutroUsuario()
    {
        var usuarioId = Guid.NewGuid();
        var outroUsuarioId = Guid.NewGuid();
        var notificacao = new Notificacao(
            outroUsuarioId,
            TipoNotificacao.TrocaAceita,
            "Troca aceita",
            "Sua troca foi aceita.");
        _repositoryMock
            .Setup(repository => repository.BuscarPorIdDoUsuarioAsync(notificacao.Id, usuarioId))
            .ReturnsAsync((Notificacao?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.MarcarComoLidaAsync(usuarioId, notificacao.Id));

        Assert.False(notificacao.Lida);
        _repositoryMock.Verify(
            repository => repository.AtualizarAsync(It.IsAny<Notificacao>()),
            Times.Never);
    }

    [Fact]
    public async Task MarcarTodasComoLidasAsync_DeveDelegarComUsuarioAutenticado()
    {
        var usuarioId = Guid.NewGuid();

        await _service.MarcarTodasComoLidasAsync(usuarioId);

        _repositoryMock.Verify(
            repository => repository.MarcarTodasComoLidasAsync(usuarioId),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DevePersistirNotificacaoComRecurso()
    {
        var usuarioId = Guid.NewGuid();
        var recursoId = Guid.NewGuid();
        _repositoryMock
            .Setup(repository => repository.AdicionarAsync(It.IsAny<Notificacao>()))
            .Returns(Task.CompletedTask);

        var notificacao = await _service.CriarAsync(
            usuarioId,
            TipoNotificacao.ProgressoAtualizado,
            "Progresso atualizado",
            "Você avançou.",
            recursoId,
            "trilha");

        Assert.Equal(usuarioId, notificacao.UsuarioId);
        Assert.Equal(recursoId, notificacao.RecursoId);
        Assert.Equal("trilha", notificacao.RecursoTipo);
        _repositoryMock.Verify(
            repository => repository.AdicionarAsync(notificacao),
            Times.Once);
    }
}
