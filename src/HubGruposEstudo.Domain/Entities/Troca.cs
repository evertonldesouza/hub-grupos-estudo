using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Troca
{
    public Guid Id { get; private set; }
    public Guid UsuarioAId { get; private set; }
    public Guid UsuarioBId { get; private set; }
    public Guid HabilidadeAId { get; private set; }
    public Guid HabilidadeBId { get; private set; }
    public Guid TrilhaAId { get; private set; }
    public Guid TrilhaBId { get; private set; }
    public StatusTroca Status { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public Usuario UsuarioA { get; private set; } = null!;
    public Usuario UsuarioB { get; private set; } = null!;
    public Habilidade HabilidadeA { get; private set; } = null!;
    public Habilidade HabilidadeB { get; private set; } = null!;
    public Trilha TrilhaA { get; private set; } = null!;
    public Trilha TrilhaB { get; private set; } = null!;

    protected Troca() { }

    public Troca(Guid usuarioAId, Guid usuarioBId, Guid habilidadeAId, Guid habilidadeBId,
                 Guid trilhaAId, Guid trilhaBId)
    {
        Id = Guid.NewGuid();
        UsuarioAId = usuarioAId;
        UsuarioBId = usuarioBId;
        HabilidadeAId = habilidadeAId;
        HabilidadeBId = habilidadeBId;
        TrilhaAId = trilhaAId;
        TrilhaBId = trilhaBId;
        Status = StatusTroca.Pendente;
        CriadoEm = DateTime.UtcNow;
    }

    public void Ativar()
    {
        if (Status != StatusTroca.Pendente)
            throw new InvalidOperationException("Apenas trocas pendentes podem ser ativadas");

        Status = StatusTroca.Ativa;
    }

    public void Concluir()
    {
        if (Status != StatusTroca.Ativa)
            throw new InvalidOperationException("Apenas trocas ativas podem ser concluídas");

        Status = StatusTroca.Concluida;
    }
}
