using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = null!;
    public PerfilUsuario Perfil { get; private set; }
    public string? Bio { get; private set; }
    public int Nivel { get; private set; }
    public int Xp { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public ICollection<Trilha> TrilhasCriadas { get; private set; } = new List<Trilha>();
    public ICollection<Inscricao> Inscricoes { get; private set; } = new List<Inscricao>();
    public ICollection<ConquistaUsuario> Conquistas { get; private set; } = new List<ConquistaUsuario>();
    public ICollection<UsuarioHabilidade> Habilidades { get; private set; } = new List<UsuarioHabilidade>();

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = PerfilUsuario.Membro;
        Nivel = 1;
        Xp = 0;
        CriadoEm = DateTime.UtcNow;
    }

    public void AdicionarXp(int quantidade)
    {
        Xp += quantidade;
        Nivel = (Xp / 100) + 1;
    }

    public void AtualizarBio(string bio)
    {
        Bio = bio;
    }
}
