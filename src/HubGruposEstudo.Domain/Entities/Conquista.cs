namespace HubGruposEstudo.Domain.Entities;

public class Conquista
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string Icone { get; private set; } = null!;
    public string Criterio { get; private set; } = null!;

    protected Conquista() { }

    public Conquista(string nome, string descricao, string icone, string criterio)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
        Icone = icone;
        Criterio = criterio;
    }
}
