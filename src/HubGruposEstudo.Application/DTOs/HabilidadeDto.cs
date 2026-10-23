using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class CriarHabilidadeDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    [StringLength(50, MinimumLength = 2)]
    public string Categoria { get; set; } = null!;
}

public class HabilidadeResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}
