namespace HubGruposEstudo.Application.DTOs;

public class UsuarioPublicoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Bio { get; set; }
    public int Nivel { get; set; }
    public int Xp { get; set; }
    public DateTime CriadoEm { get; set; }
}
