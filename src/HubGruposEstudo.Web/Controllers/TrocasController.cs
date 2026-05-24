using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
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
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var trocas = await _service.ListarPorUsuarioAsync(usuarioId);

        var result = trocas.Select(t => new TrocaResponseDto
        {
            Id = t.Id,
            UsuarioAId = t.UsuarioAId,
            UsuarioANome = t.UsuarioA.Nome,
            UsuarioBId = t.UsuarioBId,
            UsuarioBNome = t.UsuarioB.Nome,
            HabilidadeANome = t.HabilidadeA.Nome,
            HabilidadeBNome = t.HabilidadeB.Nome,
            Status = t.Status.ToString(),
            CriadoEm = t.CriadoEm
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTrocaDto dto)
    {
        try
        {
            var usuarioAId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var troca = await _service.CriarAsync(
                usuarioAId, dto.UsuarioBId,
                dto.HabilidadeAId, dto.HabilidadeBId,
                dto.TrilhaAId, dto.TrilhaBId);

            return Created(string.Empty, new TrocaResponseDto
            {
                Id = troca.Id,
                Status = troca.Status.ToString(),
                CriadoEm = troca.CriadoEm
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        try
        {
            await _service.AtivarAsync(id);
            return Ok(new { mensagem = "Troca ativada" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/concluir")]
    public async Task<IActionResult> Concluir(Guid id)
    {
        try
        {
            await _service.ConcluirAsync(id);
            return Ok(new { mensagem = "Troca concluída" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
