using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = null!;
}
