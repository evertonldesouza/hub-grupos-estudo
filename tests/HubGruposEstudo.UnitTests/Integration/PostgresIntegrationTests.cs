using HubGruposEstudo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.UnitTests.Integration;

public class PostgresIntegrationTests
{
    [Fact]
    public async Task Banco_DeveAplicarMigrationsEAceitarConexao()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=hub_grupos_estudo;Username=hub_user;Password=hub_pass")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();

        Assert.True(await context.Database.CanConnectAsync());
        Assert.NotNull(await context.Usuarios.ToListAsync());
    }
}
