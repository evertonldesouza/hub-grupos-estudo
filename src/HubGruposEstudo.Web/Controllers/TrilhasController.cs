using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
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
    public async Task<IActionResult> Listar([FromQuery] Guid? habilidadeId)
    {
        var trilhas = habilidadeId.HasValue
            ? await _service.BuscarPorHabilidadeAsync(habilidadeId.Value)
            : await _service.ListarAsync();

        var result = trilhas.Select(t => new TrilhaResponseDto
        {
            Id = t.Id,
            Titulo = t.Titulo,
            Descricao = t.Descricao,
            NivelDificuldade = t.NivelDificuldade.ToString(),
            CriadorId = t.CriadorId,
            CriadorNome = t.Criador.Nome,
            HabilidadeId = t.HabilidadeId,
            HabilidadeNome = t.Habilidade.Nome,
            TotalEtapas = t.Etapas.Count,
            CriadoEm = t.CriadoEm
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var trilha = await _service.BuscarPorIdAsync(id);
        if (trilha is null)
            return NotFound(new { mensagem = "Trilha não encontrada" });

        return Ok(new TrilhaResponseDto
        {
            Id = trilha.Id,
            Titulo = trilha.Titulo,
            Descricao = trilha.Descricao,
            NivelDificuldade = trilha.NivelDificuldade.ToString(),
            CriadorId = trilha.CriadorId,
            CriadorNome = trilha.Criador.Nome,
            HabilidadeId = trilha.HabilidadeId,
            HabilidadeNome = trilha.Habilidade.Nome,
            TotalEtapas = trilha.Etapas.Count,
            CriadoEm = trilha.CriadoEm
        });
    }

    [HttpGet("{id:guid}/etapas")]
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
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var nivel = Enum.Parse<NivelDificuldade>(dto.Trilha.NivelDificuldade);
            var etapas = dto.Etapas.Select(e => (
                e.Titulo,
                e.Descricao,
                Enum.Parse<TipoEtapa>(e.Tipo),
                e.DuracaoEstimadaMin
            )).ToList();

            var trilha = await _service.CriarComEtapasAsync(
                usuarioId, dto.Trilha.HabilidadeId,
                dto.Trilha.Titulo, dto.Trilha.Descricao, nivel, etapas);

            return Created(string.Empty, new TrilhaResponseDto
            {
                Id = trilha.Id,
                Titulo = trilha.Titulo,
                Descricao = trilha.Descricao,
                NivelDificuldade = trilha.NivelDificuldade.ToString(),
                CriadorId = trilha.CriadorId,
                HabilidadeId = trilha.HabilidadeId,
                TotalEtapas = trilha.Etapas.Count,
                CriadoEm = trilha.CriadoEm
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
