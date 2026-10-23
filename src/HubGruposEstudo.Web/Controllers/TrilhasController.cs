using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/trilhas")]
public class TrilhasController : ControllerBase
{
    private readonly TrilhaService _service;

    public TrilhasController(TrilhaService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] Guid? habilidadeId)
    {
        var trilhas = habilidadeId.HasValue
            ? await _service.BuscarPorHabilidadeAsync(habilidadeId.Value)
            : await _service.ListarAsync();

        return Ok(trilhas.Select(Mapear));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var trilha = await _service.BuscarPorIdAsync(id);
        if (trilha is null)
            return NotFound(new { mensagem = "Trilha não encontrada" });

        return Ok(Mapear(trilha));
    }

    [HttpGet("{id:guid}/etapas")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarEtapas(Guid id)
    {
        var trilha = await _service.BuscarPorIdAsync(id);
        if (trilha is null)
            return NotFound(new { mensagem = "Trilha não encontrada" });

        var etapas = trilha.Etapas.Select(e => new EtapaResponseDto
        {
            Id = e.Id,
            Ordem = e.Ordem,
            Titulo = e.Titulo,
            Descricao = e.Descricao,
            Tipo = e.Tipo.ToString(),
            DuracaoEstimadaMin = e.DuracaoEstimadaMin
        });

        return Ok(etapas);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarTrilhaComEtapasDto dto)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        if (dto?.Trilha is null)
            return BadRequest(new { mensagem = "Dados da trilha são obrigatórios" });

        if (!Enum.TryParse<NivelDificuldade>(dto.Trilha.NivelDificuldade, true, out var nivel) ||
            !Enum.IsDefined(nivel))
        {
            return BadRequest(new { mensagem = "Nível de dificuldade inválido" });
        }

        var etapas = new List<(string Titulo, string Descricao, TipoEtapa Tipo, int Duracao)>();
        foreach (var etapaDto in dto.Etapas ?? new List<EtapaDto>())
        {
            if (etapaDto is null)
                return BadRequest(new { mensagem = "Etapa inválida" });

            if (!Enum.TryParse<TipoEtapa>(etapaDto.Tipo, true, out var tipo) ||
                !Enum.IsDefined(tipo))
            {
                return BadRequest(new { mensagem = $"Tipo da etapa '{etapaDto.Titulo}' é inválido" });
            }

            etapas.Add((etapaDto.Titulo, etapaDto.Descricao, tipo, etapaDto.DuracaoEstimadaMin));
        }

        try
        {
            var trilha = await _service.CriarComEtapasAsync(
                usuarioId,
                dto.Trilha.HabilidadeId,
                dto.Trilha.Titulo,
                dto.Trilha.Descricao,
                nivel,
                etapas);

            return CreatedAtAction(nameof(BuscarPorId), new { id = trilha.Id }, Mapear(trilha));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private static TrilhaResponseDto Mapear(Trilha trilha)
    {
        return new TrilhaResponseDto
        {
            Id = trilha.Id,
            Titulo = trilha.Titulo,
            Descricao = trilha.Descricao,
            NivelDificuldade = trilha.NivelDificuldade.ToString(),
            CriadorId = trilha.CriadorId,
            CriadorNome = trilha.Criador?.Nome ?? string.Empty,
            HabilidadeId = trilha.HabilidadeId,
            HabilidadeNome = trilha.Habilidade?.Nome ?? string.Empty,
            TotalEtapas = trilha.Etapas.Count,
            CriadoEm = trilha.CriadoEm
        };
    }
}
