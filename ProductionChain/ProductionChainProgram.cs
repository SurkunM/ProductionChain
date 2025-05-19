using Microsoft.EntityFrameworkCore;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

namespace ProductionChain
{
    public class ProductionChainProgram//TODO: 1. создать данные бд, создать handler-ы (начато) надо в DI
    {
        public static void Main(string[] args)//TODO: 2. Создать фронтенд
        {
            var builder = WebApplication.CreateBuilder(args);//TODO: 3. Реализовать методы в серверной части для получения данных(Задача. кто делает. стадия пайка)

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
            builder.Services.AddTransient<IEmployeeStatusesRepository, EmployeeStatusesRepository>();

            builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
            builder.Services.AddTransient<IProductionStagesRepository, ProductionStagesRepository>();

            builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
            builder.Services.AddTransient<IWarehouseRepository, WarehouseRepository>();

            builder.Services.AddTransient<IProductionHistoryRepository, ProductionHistoryRepository>();
            builder.Services.AddTransient<IProductionOrdersRepository, ProductionOrdersRepository>();
            builder.Services.AddTransient<IProductionTasksRepository, ProductionTasksRepository>();

            builder.Services.AddTransient<CreateEmployeeHandler>();
            builder.Services.AddTransient<CreateOrderHandler>();
            builder.Services.AddTransient<CreateProductHandler>();
            builder.Services.AddTransient<CreateProductionHistoryHandler>();
            builder.Services.AddTransient<CreateProductionOrderHandler>();
            builder.Services.AddTransient<CreateProductionTaskHandler>();
            builder.Services.AddTransient<AddProductToWarehouseHandler>();

            builder.Services.AddTransient<GetEmployeesHandler>();
            builder.Services.AddTransient<GetOrdersHandler>();
            builder.Services.AddTransient<GetProductsHandler>();
            builder.Services.AddTransient<GetProductionHistoriesHandler>();
            builder.Services.AddTransient<GetProductionOrdersHandler>();
            builder.Services.AddTransient<GetProductionTasksHandler>();
            builder.Services.AddTransient<GetProductsToWarehouseHandler>();

            builder.Services.AddTransient<UpdateEmployeeHandler>();
            builder.Services.AddTransient<UpdateOrderHandler>();
            builder.Services.AddTransient<UpdateProductHandler>();
            builder.Services.AddTransient<UpdateProductionHistoryHandler>();
            builder.Services.AddTransient<UpdateProductionOrderHandler>();
            builder.Services.AddTransient<UpdateProductionTaskHandler>();
            builder.Services.AddTransient<UpdateProductToWarehouseHandler>();

            builder.Services.AddTransient<DeleteEmployeeHandler>();
            builder.Services.AddTransient<DeleteOrderHandler>();
            builder.Services.AddTransient<DeleteProductHandler>();
            builder.Services.AddTransient<DeleteProductionHistoryHandler>();
            builder.Services.AddTransient<DeleteProductionOrderHandler>();
            builder.Services.AddTransient<DeleteProductionTaskHandler>();
            builder.Services.AddTransient<DeleteProductToWarehouseHandler>();

            //jobs ...


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
                app.UseExceptionHandler("/Home/Error");
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
