using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/conquistas")]
[Authorize]
public class ConquistasController : ControllerBase
{
    private readonly ConquistaService _service;

    public ConquistasController(ConquistaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var conquistas = await _service.ListarAsync();
        return Ok(conquistas.Select(Mapear));
    }

    [HttpGet("minhas")]
    public async Task<IActionResult> Minhas()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var conquistas = await _service.ListarDoUsuarioAsync(usuarioId);
        return Ok(conquistas.Select(MapearUsuario));
    }

    private static ConquistaResponseDto Mapear(Conquista conquista)
    {
        return new ConquistaResponseDto
        {
            Id = conquista.Id,
            Nome = conquista.Nome,
            Descricao = conquista.Descricao,
            Icone = conquista.Icone,
            Criterio = conquista.Criterio
        };
    }

    private static ConquistaUsuarioResponseDto MapearUsuario(ConquistaUsuario conquista)
    {
        return new ConquistaUsuarioResponseDto
        {
            Id = conquista.Id,
            ConquistaId = conquista.ConquistaId,
            Nome = conquista.Conquista?.Nome ?? string.Empty,
            Descricao = conquista.Conquista?.Descricao ?? string.Empty,
            Icone = conquista.Conquista?.Icone ?? string.Empty,
            DataObtida = conquista.DataObtida
        };
    }
}
