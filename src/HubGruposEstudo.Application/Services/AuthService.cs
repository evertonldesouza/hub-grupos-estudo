using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HubGruposEstudo.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task RegistrarAsync(RegistrarUsuarioDto dto)
    {
        var emailExiste = await _usuarioRepository.ExistePorEmailAsync(dto.Email);
        if (emailExiste)
            throw new InvalidOperationException("Email já cadastrado");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        var usuario = new Usuario(dto.Nome, dto.Email, senhaHash);

        await _usuarioRepository.AdicionarAsync(usuario);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepository.BuscarPorEmailAsync(dto.Email);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        return GerarToken(usuario);
    }

    private string GerarToken(Usuario usuario)
    {
        var secretKey = _configuration["Jwt:SecretKey"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiracao = DateTime.UtcNow.AddHours(
            double.Parse(_configuration["Jwt:ExpiracaoHoras"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiracao,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}