using HubGruposEstudo.Application.DTOs;
using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Interfaces;

namespace HubGruposEstudo.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
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
}