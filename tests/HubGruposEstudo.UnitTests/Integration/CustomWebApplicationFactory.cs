using HubGruposEstudo.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HubGruposEstudo.UnitTests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "chave-de-integracao-com-mais-de-32-caracteres-2026",
                ["Jwt:Issuer"] = "HubGruposEstudo",
                ["Jwt:Audience"] = "HubGruposEstudo",
                ["Jwt:ExpiracaoHoras"] = "8",
                ["Database:ApplyMigrations"] = "false"
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("HubGruposEstudoIntegration"));
        });
    }
}
