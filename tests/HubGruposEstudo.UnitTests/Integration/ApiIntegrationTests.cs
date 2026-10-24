using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HubGruposEstudo.UnitTests.Integration;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_DeveRetornarOk_ComBancoDisponivel()
    {
        using var client = CreateClient();
        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Auth_DeveRegistrarEAutenticarUsuario()
    {
        using var client = CreateClient();
        var email = $"integracao-{Guid.NewGuid():N}@example.com";
        var senha = "Senha@123456";

        var registro = await client.PostAsJsonAsync("/api/auth/registrar", new
        {
            nome = "Usuário de Integração",
            email,
            senha
        });
        Assert.Equal(HttpStatusCode.Created, registro.StatusCode);

        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            senha
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var payload = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = payload.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var perfil = await client.GetAsync("/api/usuarios/me");
        Assert.Equal(HttpStatusCode.OK, perfil.StatusCode);
    }

    private HttpClient CreateClient()
    {
        try
        {
            return _factory.CreateClient();
        }
        catch (Exception exception)
        {
            throw new Xunit.Sdk.XunitException(
                $"Falha ao iniciar a API de integração:\n{exception}");
        }
    }
}
