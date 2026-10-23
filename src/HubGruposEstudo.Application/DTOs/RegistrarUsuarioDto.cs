using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class RegistrarUsuarioDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = null!;
}
