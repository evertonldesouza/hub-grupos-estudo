using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
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
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var inscricoes = await _service.ListarPorUsuarioAsync(usuarioId);

        var result = inscricoes.Select(i =>
        {
            var totalEtapas = i.Trilha?.Etapas.Count ?? 0;
            return new InscricaoResponseDto
            {
                Id = i.Id,
                TrilhaId = i.TrilhaId,
                TrilhaTitulo = i.Trilha?.Titulo ?? "",
                EtapaAtual = i.EtapaAtual,
                Concluida = i.Concluida,
                ProgressoPct = totalEtapas > 0
                    ? Math.Round((double)(i.Concluida ? totalEtapas : i.EtapaAtual - 1) / totalEtapas * 100, 1)
                    : 0,
                DataInicio = i.DataInicio,
                DataConclusao = i.DataConclusao
            };
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Inscrever([FromBody] InscreverDto dto)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var inscricao = await _service.InscreverAsync(usuarioId, dto.TrilhaId);

            return Created(string.Empty, new InscricaoResponseDto
            {
                Id = inscricao.Id,
                TrilhaId = inscricao.TrilhaId,
                EtapaAtual = inscricao.EtapaAtual,
                Concluida = inscricao.Concluida,
                DataInicio = inscricao.DataInicio
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/avancar")]
    public async Task<IActionResult> Avancar(Guid id)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.AvancarEtapaAsync(usuarioId, id);
            return Ok(new { mensagem = "Etapa avançada" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.ConcluirAsync(usuarioId, id);
            return Ok(new { mensagem = "Trilha concluída" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}
