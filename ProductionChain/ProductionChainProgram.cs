using Microsoft.EntityFrameworkCore;
using NLog.Web;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.Settings;
using ProductionChain.Middleware;
using ProductionChain.ProductionChainApiConfiguration;

namespace ProductionChain;

public class ProductionChainProgram
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var connectionString = builder.Configuration.GetConnectionString("ProductionChainConnection");

        services.ConfigureProductionChainDbContext(connectionString);
        services.ConfigureProductionChainIdentity();

        services.AddAuthorizationBuilder();

        services.ConfigureProductionChainJwtBearer(builder.Configuration);

        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();
        builder.Services.AddMemoryCache();

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));

        services.ConfigureProductionChainDIServices();
        services.ConfigureProductionChainDIRepositories();
        services.ConfigureProductionChainDIHandlers();

        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        var app = builder.Build();

        await app.InitializeProductionChainDbAsync();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<TaskQueueNotificationHub>("/TaskQueueNotificationHub");

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}