using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;

namespace ProductionChain
{
    public class ProductionChainProgram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProductionChainDbContext>(options =>
            {
                options
                    .UseSqlServer(builder.Configuration.GetConnectionString("ProductionChainConnection"))
                    .UseLazyLoadingProxies();
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            builder.Services.AddTransient<DbInitializer>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
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
