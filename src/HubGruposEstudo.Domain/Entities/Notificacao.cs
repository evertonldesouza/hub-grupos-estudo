using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public TipoNotificacao Tipo { get; private set; }
    public string Titulo { get; private set; } = null!;
    public string Mensagem { get; private set; } = null!;
    public string? RecursoTipo { get; private set; }
    public Guid? RecursoId { get; private set; }
    public bool Lida { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public DateTime? LidaEm { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    protected Notificacao() { }

    public Notificacao(
        Guid usuarioId,
        TipoNotificacao tipo,
        string titulo,
        string mensagem,
        Guid? recursoId = null,
        string? recursoTipo = null)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário é obrigatório", nameof(usuarioId));

        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título é obrigatório", nameof(titulo));

        if (string.IsNullOrWhiteSpace(mensagem))
            throw new ArgumentException("Mensagem é obrigatória", nameof(mensagem));

        if (recursoTipo?.Length > 50)
            throw new ArgumentException("Tipo do recurso pode ter no máximo 50 caracteres", nameof(recursoTipo));

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Tipo = tipo;
        Titulo = titulo.Trim();
        Mensagem = mensagem.Trim();
        RecursoId = recursoId;
        RecursoTipo = string.IsNullOrWhiteSpace(recursoTipo) ? null : recursoTipo.Trim();
        Lida = false;
        CriadaEm = DateTime.UtcNow;
    }

    public void MarcarComoLida()
    {
        if (Lida)
            return;

        Lida = true;
        LidaEm = DateTime.UtcNow;
    }
}
