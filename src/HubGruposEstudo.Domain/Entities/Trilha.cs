using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Trilha
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public NivelDificuldade NivelDificuldade { get; private set; }
    public Guid CriadorId { get; private set; }
    public Guid HabilidadeId { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public Usuario Criador { get; private set; } = null!;
    public Habilidade Habilidade { get; private set; } = null!;
    public ICollection<Etapa> Etapas { get; private set; } = new List<Etapa>();

    protected Trilha() { }

    public Trilha(string titulo, string descricao, NivelDificuldade nivelDificuldade,
                  Guid criadorId, Guid habilidadeId)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Descricao = descricao;
        NivelDificuldade = nivelDificuldade;
        CriadorId = criadorId;
        HabilidadeId = habilidadeId;
        CriadoEm = DateTime.UtcNow;
    }

    public void AdicionarEtapa(Etapa etapa)
    {
        Etapas.Add(etapa);
    }
}
