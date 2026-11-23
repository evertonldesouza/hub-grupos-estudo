using HubGruposEstudo.Domain.Entities;

namespace HubGruposEstudo.Application.DTOs;

public class NotificacaoResponseDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Mensagem { get; set; } = null!;
    public string? RecursoTipo { get; set; }
    public Guid? RecursoId { get; set; }
    public bool Lida { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime? LidaEm { get; set; }
}

public class NotificacaoPaginaResponseDto
{
    public List<NotificacaoResponseDto> Itens { get; set; } = new();
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalNaoLidas { get; set; }
}

public static class NotificacaoDtoMapper
{
    public static NotificacaoResponseDto ToResponse(this Notificacao notificacao)
    {
        return new NotificacaoResponseDto
        {
            Id = notificacao.Id,
            Tipo = notificacao.Tipo.ToString(),
            Titulo = notificacao.Titulo,
            Mensagem = notificacao.Mensagem,
            RecursoTipo = notificacao.RecursoTipo,
            RecursoId = notificacao.RecursoId,
            Lida = notificacao.Lida,
            CriadaEm = notificacao.CriadaEm,
            LidaEm = notificacao.LidaEm
        };
    }
}
