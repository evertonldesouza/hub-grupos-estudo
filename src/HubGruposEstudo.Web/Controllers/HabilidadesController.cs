using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/habilidades")]
public class HabilidadesController : ControllerBase
{
    private readonly HabilidadeService _service;

    public HabilidadesController(HabilidadeService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var habilidades = await _service.ListarAsync();
        var result = habilidades.Select(Mapear);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var habilidade = await _service.BuscarPorIdAsync(id);
        if (habilidade is null)
            return NotFound(new { mensagem = "Habilidade não encontrada" });

        return Ok(Mapear(habilidade));
    }

    [HttpPost]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<IActionResult> Criar([FromBody] CriarHabilidadeDto dto)
    {
        try
        {
            var habilidade = await _service.CriarAsync(dto.Nome, dto.Categoria);
            return CreatedAtAction(nameof(BuscarPorId), new { id = habilidade.Id }, Mapear(habilidade));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    private static HabilidadeResponseDto Mapear(Habilidade habilidade)
    {
        return new HabilidadeResponseDto
        {
            Id = habilidade.Id,
            Nome = habilidade.Nome,
            Categoria = habilidade.Categoria
        };
    }
}
