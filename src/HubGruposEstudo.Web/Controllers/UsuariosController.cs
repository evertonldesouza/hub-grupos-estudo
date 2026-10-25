using System.Security.Claims;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using HubGruposEstudo.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuariosController(UsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? busca)
    {
        var usuarios = await _service.ListarAsync(busca);
        return Ok(usuarios.Select(Mapear));
    }

    [HttpGet("me")]
    public async Task<IActionResult> MeuPerfil()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var usuario = await _service.BuscarPorIdAsync(usuarioId);
        return usuario is null
            ? NotFound(new { mensagem = "Usuário não encontrado" })
            : Ok(Mapear(usuario));
    }

    [HttpPut("me")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] AtualizarPerfilDto dto)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var usuario = await _service.AtualizarPerfilAsync(usuarioId, dto);
            return Ok(Mapear(usuario));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("me/habilidades")]
    public async Task<IActionResult> MinhasHabilidades()
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        var habilidades = await _service.ListarHabilidadesAsync(usuarioId);
        return Ok(habilidades.Select(MapearHabilidade));
    }

    [HttpPut("me/habilidades")]
    public async Task<IActionResult> AtualizarMinhasHabilidades(
        [FromBody] AtualizarHabilidadesDto dto)
    {
        if (!TryGetUsuarioId(out var usuarioId))
            return Unauthorized(new { mensagem = "Usuário não identificado" });

        try
        {
            var habilidades = await _service.AtualizarHabilidadesAsync(
                usuarioId, dto?.HabilidadeIds ?? new List<Guid>());
            return Ok(habilidades.Select(MapearHabilidade));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var usuario = await _service.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado" });

        return Ok(Mapear(usuario));
    }

    [HttpGet("{id:guid}/habilidades")]
    public async Task<IActionResult> Habilidades(Guid id)
    {
        var usuario = await _service.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado" });

        return Ok(usuario.Habilidades
            .Where(relacionamento => relacionamento.Habilidade is not null)
            .Select(relacionamento => MapearHabilidade(relacionamento.Habilidade)));
    }

    private bool TryGetUsuarioId(out Guid usuarioId)
    {
        return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);
    }

    private static UsuarioPublicoDto Mapear(Usuario usuario)
    {
        return new UsuarioPublicoDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Bio = usuario.Bio,
            Nivel = usuario.Nivel,
            Xp = usuario.Xp,
            CriadoEm = usuario.CriadoEm,
            Habilidades = usuario.Habilidades
                .Where(relacionamento => relacionamento.Habilidade is not null)
                .Select(relacionamento => MapearHabilidade(relacionamento.Habilidade))
                .ToList()
        };
    }

    private static HabilidadeResumoDto MapearHabilidade(Habilidade habilidade)
    {
        return new HabilidadeResumoDto
        {
            Id = habilidade.Id,
            Nome = habilidade.Nome,
            Categoria = habilidade.Categoria
        };
    }
}
