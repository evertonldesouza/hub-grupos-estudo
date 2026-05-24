using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
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
    public async Task<IActionResult> Listar()
    {
        var habilidades = await _service.ListarAsync();
        var result = habilidades.Select(h => new HabilidadeResponseDto
        {
            Id = h.Id,
            Nome = h.Nome,
            Categoria = h.Categoria
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var habilidade = await _service.BuscarPorIdAsync(id);
        if (habilidade is null)
            return NotFound(new { mensagem = "Habilidade não encontrada" });

        return Ok(new HabilidadeResponseDto
        {
            Id = habilidade.Id,
            Nome = habilidade.Nome,
            Categoria = habilidade.Categoria
        });
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarHabilidadeDto dto)
    {
        try
        {
            var habilidade = await _service.CriarAsync(dto.Nome, dto.Categoria);
            return Created(string.Empty, new HabilidadeResponseDto
            {
                Id = habilidade.Id,
                Nome = habilidade.Nome,
                Categoria = habilidade.Categoria
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }
}
