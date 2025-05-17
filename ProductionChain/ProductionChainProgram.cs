using Microsoft.EntityFrameworkCore;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;

namespace ProductionChain
{
    public class ProductionChainProgram//TODO: 1. создать данные бд
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

            //handlers ...

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
