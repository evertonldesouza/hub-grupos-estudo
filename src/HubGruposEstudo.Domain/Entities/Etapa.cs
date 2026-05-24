using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Etapa
{
    public Guid Id { get; private set; }
    public Guid TrilhaId { get; private set; }
    public int Ordem { get; private set; }
    public string Titulo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public TipoEtapa Tipo { get; private set; }
    public int DuracaoEstimadaMin { get; private set; }

    public Trilha Trilha { get; private set; } = null!;

    protected Etapa() { }

    public Etapa(Guid trilhaId, int ordem, string titulo, string descricao,
                 TipoEtapa tipo, int duracaoEstimadaMin)
    {
        Id = Guid.NewGuid();
        TrilhaId = trilhaId;
        Ordem = ordem;
        Titulo = titulo;
        Descricao = descricao;
        Tipo = tipo;
        DuracaoEstimadaMin = duracaoEstimadaMin;
    }
}
