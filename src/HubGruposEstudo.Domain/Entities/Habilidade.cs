namespace HubGruposEstudo.Domain.Entities;

public class Habilidade
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Categoria { get; private set; } = null!;

    protected Habilidade() { }

    public Habilidade(string nome, string categoria)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Categoria = categoria;
    }
}
