using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;
using ProductionChain.Model.BasicEntities;
using System.Security.Claims;

namespace ProductionChain.ProductionChainAppConfiguration.AppDbContext;

public static class ProductionChainDbContextConfiguration
{
    public static void ConfigureProductionChainDbContext(this IServiceCollection services, string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<ProductionChainDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Scoped, ServiceLifetime.Transient);
    }

    public static void ConfigureProductionChainIdentity(this IServiceCollection services)
    {
        services.AddIdentity<Account, IdentityRole<int>>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ProductionChainDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        });
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
