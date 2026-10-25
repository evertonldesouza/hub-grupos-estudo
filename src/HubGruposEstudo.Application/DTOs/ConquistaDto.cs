namespace HubGruposEstudo.Application.DTOs;

public class ConquistaResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Icone { get; set; } = null!;
    public string Criterio { get; set; } = null!;
}

public class ConquistaUsuarioResponseDto
{
    public Guid Id { get; set; }
    public Guid ConquistaId { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Icone { get; set; } = null!;
    public DateTime DataObtida { get; set; }
}
