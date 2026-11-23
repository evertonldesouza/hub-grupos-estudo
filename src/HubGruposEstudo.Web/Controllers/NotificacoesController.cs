using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/notificacoes")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly INotificacaoService _service;

    public NotificacoesController(INotificacaoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] bool apenasNaoLidas = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var paginaResult = await _service.ListarAsync(
            usuarioId, apenasNaoLidas, pagina, tamanhoPagina);

        return Ok(new NotificacaoPaginaResponseDto
        {
            Itens = paginaResult.Itens.Select(item => item.ToResponse()).ToList(),
            Pagina = paginaResult.Pagina,
            TamanhoPagina = paginaResult.TamanhoPagina,
            TotalNaoLidas = paginaResult.TotalNaoLidas
        });
    }

    [HttpGet("nao-lidas/quantidade")]
    public async Task<IActionResult> ContarNaoLidas()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        return Ok(new { total = await _service.ContarNaoLidasAsync(usuarioId) });
    }

    [HttpPost("{id:guid}/ler")]
    public async Task<IActionResult> MarcarComoLida(Guid id)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            await _service.MarcarComoLidaAsync(usuarioId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("ler-todas")]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        await _service.MarcarTodasComoLidasAsync(usuarioId);
        return NoContent();
    }

    private bool TryGetUsuarioId(out Guid usuarioId)
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);
    }
}
