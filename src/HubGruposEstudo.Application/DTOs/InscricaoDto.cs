namespace HubGruposEstudo.Application.DTOs;

public class InscreverDto
{
    public Guid TrilhaId { get; set; }
}

public class InscricaoResponseDto
{
    public Guid Id { get; set; }
    public Guid TrilhaId { get; set; }
    public string TrilhaTitulo { get; set; } = null!;
    public int EtapaAtual { get; set; }
    public bool Concluida { get; set; }
    public double ProgressoPct { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataConclusao { get; set; }
}
