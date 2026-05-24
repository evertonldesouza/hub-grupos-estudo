using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class CriarHabilidadeDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100)]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    [MaxLength(50)]
    public string Categoria { get; set; } = null!;
}

public class HabilidadeResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}
