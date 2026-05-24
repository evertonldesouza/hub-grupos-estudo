namespace HubGruposEstudo.Domain.Entities;

public class ConquistaUsuario
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid ConquistaId { get; private set; }
    public DateTime DataObtida { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Conquista Conquista { get; private set; } = null!;

    protected ConquistaUsuario() { }

    public ConquistaUsuario(Guid usuarioId, Guid conquistaId)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        ConquistaId = conquistaId;
        DataObtida = DateTime.UtcNow;
    }
}
