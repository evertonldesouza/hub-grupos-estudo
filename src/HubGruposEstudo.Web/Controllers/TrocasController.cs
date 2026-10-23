using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/trocas")]
[Authorize]
public class TrocasController : ControllerBase
{
    private readonly TrocaService _service;

    public TrocasController(TrocaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var trocas = await _service.ListarPorUsuarioAsync(usuarioId);
        return Ok(trocas.Select(Mapear));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTrocaDto dto)
    {
        if (!TryGetUsuarioId(out var usuarioAId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var troca = await _service.CriarAsync(
                usuarioAId,
                dto.UsuarioBId,
                dto.HabilidadeAId,
                dto.HabilidadeBId,
                dto.TrilhaAId,
                dto.TrilhaBId);

            return CreatedAtAction(nameof(Listar), null, Mapear(troca));
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

    [HttpPost("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var troca = await _service.AtivarAsync(id, usuarioId);
            return Ok(Mapear(troca));
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
            var troca = await _service.ConcluirAsync(id, usuarioId);
            return Ok(Mapear(troca));
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

    private static TrocaResponseDto Mapear(Troca troca)
    {
        return new TrocaResponseDto
        {
            Id = troca.Id,
            UsuarioAId = troca.UsuarioAId,
            UsuarioANome = troca.UsuarioA?.Nome ?? string.Empty,
            UsuarioBId = troca.UsuarioBId,
            UsuarioBNome = troca.UsuarioB?.Nome ?? string.Empty,
            HabilidadeANome = troca.HabilidadeA?.Nome ?? string.Empty,
            HabilidadeBNome = troca.HabilidadeB?.Nome ?? string.Empty,
            Status = troca.Status.ToString(),
            CriadoEm = troca.CriadoEm
        };
    }
}
