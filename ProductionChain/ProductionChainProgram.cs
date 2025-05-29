using Microsoft.EntityFrameworkCore;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;

namespace ProductionChain
{
    public class ProductionChainProgram
    {
        public static void Main(string[] args)//TODO: 1. Сделать методы Delete в API
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
            //builder.Services.AddTransient<IEmployeeStatusesRepository, EmployeeStatusesRepository>();

            builder.Services.AddTransient<IProductsRepository, ProductsRepository>();

            builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();

            builder.Services.AddTransient<IProductionAssemblyHistoryRepository, ProductionAssemblyHistoryRepository>();
            builder.Services.AddTransient<IProductionAssemblyOrdersRepository, ProductionAssemblyOrdersRepository>();
            builder.Services.AddTransient<IProductionAssemblyTasksRepository, ProductionAssemblyTasksRepository>();
            builder.Services.AddTransient<IProductionAssemblyWarehouseRepository, ProductionAssemblyWarehouseRepository>();
            builder.Services.AddTransient<IComponentsWarehouseRepository, ComponentsWarehouseRepository>();

            builder.Services.AddTransient<CreateOrderHandler>();
            builder.Services.AddTransient<CreateProductionHistoryHandler>();
            builder.Services.AddTransient<CreateProductionOrderHandler>();
            builder.Services.AddTransient<CreateProductionTaskHandler>();
            builder.Services.AddTransient<CreateAssemblyWarehouseItemHandler>();

            builder.Services.AddTransient<GetEmployeesHandler>();
            //builder.Services.AddTransient<GetEmployeesStatusesHandler>(); Удалить
            builder.Services.AddTransient<GetOrdersHandler>();
            builder.Services.AddTransient<GetProductsHandler>();

            builder.Services.AddTransient<GetProductionHistoriesHandler>();
            builder.Services.AddTransient<GetProductionOrdersHandler>();
            builder.Services.AddTransient<GetProductionTasksHandler>();
            builder.Services.AddTransient<GetAssemblyWarehouseItemsHandler>();
            builder.Services.AddTransient<GetComponentsWarehouseItemsHandler>();

            builder.Services.AddTransient<UpdateEmployeeStatusHandler>();//Удалить
            builder.Services.AddTransient<UpdateOrderHandler>();
            builder.Services.AddTransient<UpdateProductionOrderHandler>();
            builder.Services.AddTransient<UpdateProductionTaskHandler>();
            builder.Services.AddTransient<UpdateAssemblyWarehouseItemHandler>();

            builder.Services.AddTransient<DeleteOrderHandler>();
            builder.Services.AddTransient<DeleteProductionHistoryHandler>();
            builder.Services.AddTransient<DeleteProductionOrderHandler>();
            builder.Services.AddTransient<DeleteProductionTaskHandler>();
            builder.Services.AddTransient<DeleteAssemblyWarehouseItemHandler>();

            //jobs ...

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                    //dbInitializer.Initialize();
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
                //app.UseExceptionHandler("/Home/Error");
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
}
