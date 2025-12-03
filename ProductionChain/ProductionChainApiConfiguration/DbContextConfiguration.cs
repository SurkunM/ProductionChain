using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;

namespace ProductionChain.ProductionChainApiConfiguration;

public static class DbContextConfiguration
{
    public static void ConfigureProductionChainDbContext(this IServiceCollection services, string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<ProductionChainDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Scoped, ServiceLifetime.Transient);
    }

    public static async Task InitializeProductionChainDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        try
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            await dbInitializer.Initialize();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<ProductionChainProgram>>();
            logger.LogError(ex, "При создании базы данных произошла ошибка.");

            throw;
        }
    }
}
