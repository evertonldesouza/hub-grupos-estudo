using System.Globalization;
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
        ArgumentNullException.ThrowIfNull(dto);

        var nome = dto.Nome?.Trim();
        var email = NormalizarEmail(dto.Email);
        var senha = dto.Senha;

        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new InvalidOperationException("Senha deve ter no mínimo 6 caracteres");

        var emailExiste = await _usuarioRepository.ExistePorEmailAsync(email);
        if (emailExiste)
            throw new InvalidOperationException("Email já cadastrado");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
        var usuario = new Usuario(nome, email, senhaHash);

        await _usuarioRepository.AdicionarAsync(usuario);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var email = NormalizarEmail(dto.Email);
        var usuario = await _usuarioRepository.BuscarPorEmailAsync(email);
        if (usuario is null || string.IsNullOrWhiteSpace(dto.Senha) ||
            !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        return GerarToken(usuario);
    }

    private string GerarToken(Usuario usuario)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
            throw new InvalidOperationException("A chave JWT não está configurada corretamente");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            // O sub segue o padrão JWT e o NameIdentifier garante compatibilidade
            // com o acesso ao usuário nos controllers do ASP.NET Core.
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiracaoHoras = 8d;
        if (double.TryParse(_configuration["Jwt:ExpiracaoHoras"], NumberStyles.Float,
                CultureInfo.InvariantCulture, out var configurada) && configurada > 0)
        {
            expiracaoHoras = configurada;
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiracaoHoras),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string NormalizarEmail(string? email)
    {
        return email?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
