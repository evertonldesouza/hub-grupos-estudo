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
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("Usuário é obrigatório", nameof(usuarioId));

        if (trilhaId == Guid.Empty)
            throw new ArgumentException("Trilha é obrigatória", nameof(trilhaId));

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TrilhaId = trilhaId;
        EtapaAtual = 1;
        Concluida = false;
        DataInicio = DateTime.UtcNow;
    }

    public void AssociarTrilha(Trilha trilha)
    {
        ArgumentNullException.ThrowIfNull(trilha);
        Trilha = trilha;
    }

    public void AvancarEtapa(int totalEtapas)
    {
        if (Concluida)
            throw new InvalidOperationException("Trilha já concluída");

        if (totalEtapas <= 0)
            throw new InvalidOperationException("A trilha não possui etapas válidas");

        // A etapa é 1-based. Ao avançar da última etapa, a inscrição é concluída
        // automaticamente para evitar progresso acima de 100%.
        if (EtapaAtual >= totalEtapas)
        {
            Concluir();
            return;
        }

        EtapaAtual++;
    }

    public void Concluir()
    {
        if (Concluida)
            throw new InvalidOperationException("Trilha já concluída");

        Concluida = true;
        DataConclusao = DateTime.UtcNow;
    }
}
