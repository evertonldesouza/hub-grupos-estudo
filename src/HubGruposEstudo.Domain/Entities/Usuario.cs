using HubGruposEstudo.Domain.Enums;

namespace HubGruposEstudo.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = null!;
    public PerfilUsuario Perfil { get; private set; }
    public DateTime CriadoEm { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = PerfilUsuario.Membro;
        CriadoEm = DateTime.UtcNow;
    }
}