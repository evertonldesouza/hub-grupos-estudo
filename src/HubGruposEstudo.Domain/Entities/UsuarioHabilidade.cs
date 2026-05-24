namespace HubGruposEstudo.Domain.Entities;

public class UsuarioHabilidade
{
    public Guid UsuarioId { get; private set; }
    public Guid HabilidadeId { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Habilidade Habilidade { get; private set; } = null!;

    protected UsuarioHabilidade() { }

    public UsuarioHabilidade(Guid usuarioId, Guid habilidadeId)
    {
        UsuarioId = usuarioId;
        HabilidadeId = habilidadeId;
    }
}
