using Microsoft.EntityFrameworkCore;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;

namespace ProductionChain;

public class ProductionChainProgram
{//TODO: изменить сообщения в лог в контроллерах! Тесты, jobs
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ProductionChainDbContext>(options =>
        {
            options
                .UseSqlServer(builder.Configuration.GetConnectionString("ProductionChainConnection"))
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Scoped, ServiceLifetime.Transient);

        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<DbInitializer>();

        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<ProductionChainDbContext>());
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

        builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>();
        builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
        builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();

        builder.Services.AddTransient<IProductionHistoryRepository, ProductionHistoryRepository>();
        builder.Services.AddTransient<IAssemblyProductionOrdersRepository, AssemblyProductionOrdersRepository>();
        builder.Services.AddTransient<IAssemblyProductionTasksRepository, AssemblyProductionTasksRepository>();
        builder.Services.AddTransient<IAssemblyProductionWarehouseRepository, AssemblyProductionWarehouseRepository>();
        builder.Services.AddTransient<IComponentsWarehouseRepository, ComponentsWarehouseRepository>();

        builder.Services.AddTransient<CreateProductionOrderHandler>();
        builder.Services.AddTransient<CreateProductionTaskHandler>();

        builder.Services.AddTransient<GetEmployeesHandler>();
        builder.Services.AddTransient<GetOrdersHandler>();
        builder.Services.AddTransient<GetProductsHandler>();

        builder.Services.AddTransient<GetProductionHistoriesHandler>();
        builder.Services.AddTransient<GetProductionOrdersHandler>();
        builder.Services.AddTransient<GetProductionTasksHandler>();
        builder.Services.AddTransient<GetAssemblyWarehouseItemsHandler>();
        builder.Services.AddTransient<GetComponentsWarehouseItemsHandler>();

        builder.Services.AddTransient<DeleteProductionOrderHandler>();
        builder.Services.AddTransient<DeleteProductionTaskHandler>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                dbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<ProductionChainProgram>>();
                logger.LogError(ex, "При создании базы данных произошла ошибка.");

                throw;
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
