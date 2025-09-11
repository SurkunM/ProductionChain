using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.BusinessLogic.Handlers.Authorization;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.DataAccess.UnitOfWork;
using ProductionChain.Middleware;
using ProductionChain.Model.BasicEntities;
using System.Security.Claims;
using System.Text;

namespace ProductionChain;

public class ProductionChainProgram//TODO:1. ������������� � ���������� ������� ��� bool ������� 2. �� ������ �������� HomeTab 
{//3. ����������� ������� �������� � Excel �������(�������� ������ ���� ���� �����) 4. �������� ����-������
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ProductionChainDbContext>(options =>
        {
            options
                .UseSqlServer(builder.Configuration.GetConnectionString("ProductionChainConnection"))
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Scoped, ServiceLifetime.Transient);

        builder.Services.AddIdentity<Account, IdentityRole<int>>()
            .AddEntityFrameworkStores<ProductionChainDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        });

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                   
                    ValidIssuer = builder.Configuration["Jwt:Issuer"], // ��� �������� ������ (���� ����������)                    
                    ValidAudience = builder.Configuration["Jwt:Audience"],// ��� ���� ������������ ����� (���� ����������)                   
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), // ���� ��� �������� �������
                    
                    ValidateIssuerSigningKey = true,// ��������� �������                    
                    ValidateIssuer = true,// ��������� ��������                    
                    ValidateAudience = true,// ��������� ����������                    
                    ValidateLifetime = true,// ��������� ���� ��������
                   
                    ClockSkew = TimeSpan.FromMinutes(5) // ���������� ����������� ������� (��� ������ ��������)
                };

                // ��� SPA ���������� ����� ��������� CORS � �������
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

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

        builder.Services.AddTransient<AccountRegisterHandler>();
        builder.Services.AddTransient<LoginHandler>();

        builder.Services.AddTransient<AccountAuthorizationHandler>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                await dbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<ProductionChainProgram>>();
                logger.LogError(ex, "��� �������� ���� ������ ��������� ������.");

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

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
