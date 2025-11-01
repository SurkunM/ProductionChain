using Microsoft.EntityFrameworkCore;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.BusinessLogic.Handlers.Authorization;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Notification;
using ProductionChain.BusinessLogic.Services;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;

namespace ProductionChain.ProductionChainAppConfiguration.AppDependencies;

public static class DependenciesConfiguration
{
    public static void ConfigureProductionChainDIServices(this IServiceCollection services)
    {
        services.AddScoped<DbInitializer>();
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ProductionChainDbContext>());

        services.AddTransient<INotificationService, SignalRNotificationService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IJwtGenerationService, JwtGenerationService>();
        services.AddSingleton<ITaskQueueService, TaskQueueService>();
    }

    public static void ConfigureProductionChainDIRepositories(this IServiceCollection services)
    {
        services.AddTransient<IEmployeesRepository, EmployeesRepository>();
        services.AddTransient<IProductsRepository, ProductsRepository>();
        services.AddTransient<IOrdersRepository, OrdersRepository>();

        services.AddTransient<IProductionHistoryRepository, ProductionHistoryRepository>();
        services.AddTransient<IAssemblyProductionOrdersRepository, AssemblyProductionOrdersRepository>();
        services.AddTransient<IAssemblyProductionTasksRepository, AssemblyProductionTasksRepository>();
        services.AddTransient<IAssemblyProductionWarehouseRepository, AssemblyProductionWarehouseRepository>();
        services.AddTransient<IComponentsWarehouseRepository, ComponentsWarehouseRepository>();
    }

    public static void ConfigureProductionChainDIHandlers(this IServiceCollection services)
    {
        services.AddTransient<CreateProductionOrderHandler>();
        services.AddTransient<CreateProductionTaskHandler>();
        services.AddTransient<AddToTaskQueueAndManagersNotificationHandler>();

        services.AddTransient<GetEmployeesHandler>();
        services.AddTransient<GetOrdersHandler>();
        services.AddTransient<GetProductsHandler>();
        services.AddTransient<GetTaskQueueHandler>();

        services.AddTransient<GetProductionHistoriesHandler>();
        services.AddTransient<GetProductionOrdersHandler>();
        services.AddTransient<GetProductionTasksHandler>();
        services.AddTransient<GetAssemblyWarehouseItemsHandler>();
        services.AddTransient<GetComponentsWarehouseItemsHandler>();

        services.AddTransient<DeleteProductionOrderHandler>();
        services.AddTransient<DeleteProductionTaskHandler>();

        services.AddTransient<AccountAuthenticationHandler>();
        services.AddTransient<AccountAuthorizationHandler>();
        services.AddTransient<RemoveEmployeeFromTaskQueueHandler>();

        services.AddTransient<NotifyEmployeeHandler>();
    }
}
