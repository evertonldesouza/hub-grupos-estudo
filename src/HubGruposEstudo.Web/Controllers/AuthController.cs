using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubGruposEstudo.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        try
        {
            await _authService.RegistrarAsync(dto);
            return Created(string.Empty, new { mensagem = "Usuário registrado com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}