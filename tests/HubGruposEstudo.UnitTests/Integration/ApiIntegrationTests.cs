using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HubGruposEstudo.UnitTests.Integration;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_DeveRetornarOk_ComBancoDisponivel()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Auth_DeveRegistrarEAutenticarUsuario()
    {
        var email = $"integracao-{Guid.NewGuid():N}@example.com";
        var senha = "Senha@123456";

        var registro = await _client.PostAsJsonAsync("/api/auth/registrar", new
        {
            nome = "Usuário de Integração",
            email,
            senha
        });
        Assert.Equal(HttpStatusCode.Created, registro.StatusCode);

        var login = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            senha
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var payload = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = payload.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var perfil = await _client.GetAsync("/api/usuarios/me");
        Assert.Equal(HttpStatusCode.OK, perfil.StatusCode);
    }
}
