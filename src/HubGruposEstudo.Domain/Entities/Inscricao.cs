namespace HubGruposEstudo.Domain.Entities;

public class Inscricao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid TrilhaId { get; private set; }
    public int EtapaAtual { get; private set; }
    public bool Concluida { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataConclusao { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Trilha Trilha { get; private set; } = null!;

    protected Inscricao() { }

    public Inscricao(Guid usuarioId, Guid trilhaId)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TrilhaId = trilhaId;
        EtapaAtual = 1;
        Concluida = false;
        DataInicio = DateTime.UtcNow;
    }

    public void AvancarEtapa()
    {
        if (Concluida)
            throw new InvalidOperationException("Trilha já concluída");

        EtapaAtual++;
    }

    public void Concluir()
    {
        Concluida = true;
        DataConclusao = DateTime.UtcNow;
    }
}
