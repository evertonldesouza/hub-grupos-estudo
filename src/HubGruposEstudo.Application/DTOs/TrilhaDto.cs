using System.ComponentModel.DataAnnotations;

namespace HubGruposEstudo.Application.DTOs;

public class CriarTrilhaDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200)]
    public string Titulo { get; set; } = null!;

    [Required(ErrorMessage = "Descrição é obrigatória")]
    [MaxLength(2000)]
    public string Descricao { get; set; } = null!;

    [Required]
    public string NivelDificuldade { get; set; } = null!;

    [Required]
    public Guid HabilidadeId { get; set; }
}

public class EtapaDto
{
    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Descricao { get; set; } = null!;

    [Required]
    public string Tipo { get; set; } = null!;

    [Range(1, 600)]
    public int DuracaoEstimadaMin { get; set; }
}

public class CriarTrilhaComEtapasDto
{
    [Required]
    public CriarTrilhaDto Trilha { get; set; } = null!;

    [Required]
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
