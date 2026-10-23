using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/inscricoes")]
[Authorize]
public class InscricoesController : ControllerBase
{
    private readonly InscricaoService _service;

    public InscricoesController(InscricaoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var inscricoes = await _service.ListarPorUsuarioAsync(usuarioId);
        return Ok(inscricoes.Select(Mapear));
    }

    [HttpPost]
    public async Task<IActionResult> Inscrever([FromBody] InscreverDto dto)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var inscricao = await _service.InscreverAsync(usuarioId, dto.TrilhaId);
            return CreatedAtAction(nameof(Listar), null, Mapear(inscricao));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/avancar")]
    public async Task<IActionResult> Avancar(Guid id)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var inscricao = await _service.AvancarEtapaAsync(usuarioId, id);
            return Ok(Mapear(inscricao));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var inscricao = await _service.ConcluirAsync(usuarioId, id);
            return Ok(Mapear(inscricao));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = ex.Message });
        }
    }

    private bool TryGetUsuarioId(out Guid usuarioId)
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);
    }

    private static InscricaoResponseDto Mapear(Inscricao inscricao)
    {
        var totalEtapas = inscricao.Trilha?.Etapas.Count ?? 0;
        var etapasConcluidas = inscricao.Concluida
            ? totalEtapas
            : Math.Clamp(inscricao.EtapaAtual - 1, 0, totalEtapas);

        return new InscricaoResponseDto
        {
            Id = inscricao.Id,
            TrilhaId = inscricao.TrilhaId,
            TrilhaTitulo = inscricao.Trilha?.Titulo ?? string.Empty,
            EtapaAtual = inscricao.EtapaAtual,
            Concluida = inscricao.Concluida,
            ProgressoPct = totalEtapas > 0
                ? Math.Round((double)etapasConcluidas / totalEtapas * 100, 1)
                : 0,
            DataInicio = inscricao.DataInicio,
            DataConclusao = inscricao.DataConclusao
        };
    }
}
