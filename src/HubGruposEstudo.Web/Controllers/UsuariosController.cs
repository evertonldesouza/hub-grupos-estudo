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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var usuario = await _service.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado" });

        return Ok(Mapear(usuario));
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
            CriadoEm = usuario.CriadoEm
        };
    }
}
