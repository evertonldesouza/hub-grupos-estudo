using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class CriarTrilhaDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(200, MinimumLength = 3)]
    public string Titulo { get; set; } = null!;

    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(2000, MinimumLength = 3)]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "Nível de dificuldade é obrigatório")]
    public string NivelDificuldade { get; set; } = null!;

    public Guid HabilidadeId { get; set; }
}

public class EtapaDto
{
    [Required(ErrorMessage = "Título da etapa é obrigatório")]
    [StringLength(200, MinimumLength = 2)]
    public string Titulo { get; set; } = null!;

    [Required(ErrorMessage = "Descrição da etapa é obrigatória")]
    [StringLength(2000, MinimumLength = 2)]
    public string Descricao { get; set; } = null!;

    [Required(ErrorMessage = "Tipo da etapa é obrigatório")]
    public string Tipo { get; set; } = null!;

    [Range(1, 600, ErrorMessage = "Duração deve estar entre 1 e 600 minutos")]
    public int DuracaoEstimadaMin { get; set; }
}

public class CriarTrilhaComEtapasDto
{
    [Required(ErrorMessage = "Dados da trilha são obrigatórios")]
    public CriarTrilhaDto Trilha { get; set; } = null!;

    [Required(ErrorMessage = "Etapas são obrigatórias")]
    [MinLength(1, ErrorMessage = "Trilha precisa de pelo menos 1 etapa")]
    public List<EtapaDto> Etapas { get; set; } = new();
}

public class TrilhaResponseDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string NivelDificuldade { get; set; } = null!;
    public Guid CriadorId { get; set; }
    public string CriadorNome { get; set; } = null!;
    public Guid HabilidadeId { get; set; }
    public string HabilidadeNome { get; set; } = null!;
    public int TotalEtapas { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class EtapaResponseDto
{
    public Guid Id { get; set; }
    public int Ordem { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public int DuracaoEstimadaMin { get; set; }
}
