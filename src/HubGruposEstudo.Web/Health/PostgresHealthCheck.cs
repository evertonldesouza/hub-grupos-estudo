using HubGruposEstudo.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HubGruposEstudo.Web.Health;

public sealed class PostgresHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PostgresHealthCheck(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var connected = await dbContext.Database.CanConnectAsync(cancellationToken);
            return connected
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("PostgreSQL não está disponível");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Não foi possível verificar o PostgreSQL",
                exception);
        }
    }
}
