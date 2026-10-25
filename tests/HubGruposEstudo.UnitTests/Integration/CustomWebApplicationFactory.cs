using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace HubGruposEstudo.UnitTests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Host=localhost;Port=5432;Database=hub_grupos_estudo;Username=hub_user;Password=hub_pass");
        builder.UseSetting("Jwt:SecretKey", "chave-de-integracao-com-mais-de-32-caracteres-2026");
        builder.UseSetting("Jwt:Issuer", "HubGruposEstudo");
        builder.UseSetting("Jwt:Audience", "HubGruposEstudo");
        builder.UseSetting("Jwt:ExpiracaoHoras", "8");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Port=5432;Database=hub_grupos_estudo;Username=hub_user;Password=hub_pass",
                ["Jwt:SecretKey"] = "chave-de-integracao-com-mais-de-32-caracteres-2026",
                ["Jwt:Issuer"] = "HubGruposEstudo",
                ["Jwt:Audience"] = "HubGruposEstudo",
                ["Jwt:ExpiracaoHoras"] = "8",
                ["Database:ApplyMigrations"] = "true"
            });
        });
    }
}
