using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class CriarTrocaDto
{
    [Required]
    public Guid UsuarioBId { get; set; }

    [Required]
    public Guid HabilidadeAId { get; set; }

    [Required]
    public Guid HabilidadeBId { get; set; }

    [Required]
    public Guid TrilhaAId { get; set; }

    [Required]
    public Guid TrilhaBId { get; set; }
}

public class TrocaResponseDto
{
    public Guid Id { get; set; }
    public Guid UsuarioAId { get; set; }
    public string UsuarioANome { get; set; } = null!;
    public Guid UsuarioBId { get; set; }
    public string UsuarioBNome { get; set; } = null!;
    public string HabilidadeANome { get; set; } = null!;
    public string HabilidadeBNome { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CriadoEm { get; set; }
}
