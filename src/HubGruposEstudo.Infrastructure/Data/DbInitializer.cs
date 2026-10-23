using HubGruposEstudo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HubGruposEstudo.Infrastructure.Data;

/// <summary>
/// Prepara o banco para uma instalação nova. As migrações continuam sendo a fonte
/// da estrutura; este componente apenas as aplica e cria o catálogo inicial.
/// </summary>
public static class DbInitializer
{
    private static readonly (string Nome, string Categoria)[] HabilidadesIniciais =
    [
        ("C# e .NET", "Backend"),
        ("JavaScript", "Frontend"),
        ("Python", "Backend"),
        ("SQL", "Dados"),
        ("Git e GitHub", "Ferramentas")
    ];

    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var aplicarMigracoes = true;
        if (bool.TryParse(configuration["Database:ApplyMigrations"], out var configurado))
            aplicarMigracoes = configurado;

        if (aplicarMigracoes)
        {
            await AplicarMigracoesComRetryAsync(context, logger, cancellationToken);
        }

        await SeedAsync(context, configuration, logger, cancellationToken);
    }

    private static async Task AplicarMigracoesComRetryAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        const int tentativas = 10;

        for (var tentativa = 1; tentativa <= tentativas; tentativa++)
        {
            try
            {
                await context.Database.MigrateAsync(cancellationToken);
                return;
            }
            catch (Exception ex) when (tentativa < tentativas)
            {
                logger.LogWarning(ex,
                    "Banco ainda não está disponível. Nova tentativa {Tentativa}/{Total} em 3 segundos.",
                    tentativa, tentativas);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        // A última tentativa é executada fora do filtro de retry para preservar a
        // exceção original e fazer a aplicação falhar com uma mensagem útil.
        await context.Database.MigrateAsync(cancellationToken);
    }

    private static async Task SeedAsync(
        AppDbContext context,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        foreach (var (nome, categoria) in HabilidadesIniciais)
        {
            var existe = await context.Habilidades
                .AnyAsync(h => h.Nome.ToUpper() == nome.ToUpper(), cancellationToken);

            if (!existe)
                await context.Habilidades.AddAsync(new Habilidade(nome, categoria), cancellationToken);
        }

        await SeedAdminAsync(context, configuration, logger, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedAdminAsync(
        AppDbContext context,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var email = configuration["BootstrapAdmin:Email"]?.Trim().ToLowerInvariant();
        var senha = configuration["BootstrapAdmin:Password"];

        // O administrador inicial é opcional. Em produção, informe as duas
        // variáveis por secret/env; não há credencial administrativa fixa no código.
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(senha))
            return;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new InvalidOperationException(
                "BootstrapAdmin exige Email e Password com pelo menos 6 caracteres");

        var existe = await context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == email, cancellationToken);

        if (existe)
            return;

        var admin = new Usuario("Administrador", email, BCrypt.Net.BCrypt.HashPassword(senha));
        admin.TornarAdministrador();

        await context.Usuarios.AddAsync(admin, cancellationToken);
        logger.LogInformation("Administrador bootstrap criado para {Email}", email);
    }
}
