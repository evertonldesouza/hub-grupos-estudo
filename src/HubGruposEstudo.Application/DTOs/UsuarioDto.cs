using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class AtualizarPerfilDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Bio pode ter no máximo 500 caracteres")]
    public string? Bio { get; set; }
}

public class AtualizarHabilidadesDto
{
    public List<Guid> HabilidadeIds { get; set; } = new();
}

public class HabilidadeResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}

public class UsuarioPublicoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Bio { get; set; }
    public int Nivel { get; set; }
    public int Xp { get; set; }
    public DateTime CriadoEm { get; set; }
    public List<HabilidadeResumoDto> Habilidades { get; set; } = new();
}
