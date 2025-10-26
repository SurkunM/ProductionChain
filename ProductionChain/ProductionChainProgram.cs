using Microsoft.EntityFrameworkCore;
using ProductionChain.BusinessLogic.Hubs;
using ProductionChain.Contracts.Settings;
using ProductionChain.Middleware;
using ProductionChain.ProductionChainAppConfiguration.AppAuthentication;
using ProductionChain.ProductionChainAppConfiguration.AppDbContext;
using ProductionChain.ProductionChainAppConfiguration.AppDependencies;
using ProductionChain.ProductionChainAppConfiguration.AppInfrastructure;

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
        services.ConfigureProductionChainCors();

        builder.Services.AddControllersWithViews();
        builder.Services.AddSignalR();
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

        services.ConfigureProductionChainDIServices();
        services.ConfigureProductionChainDIRepositories();
        services.ConfigureProductionChainDIHandlers();

        var app = builder.Build();

        await app.InitializeProductionChainDbAsync();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseCors("VueFrontend");

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