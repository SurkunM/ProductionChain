using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductionChain.Contracts.IServices;
using ProductionChain.DataAccess;
using ProductionChain.Model.BasicEntities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductionChain.ProductionChainApiConfiguration;

public static class JwtBearerConfiguration
{
    public static void ConfigureProductionChainJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),

                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/TaskQueueNotificationHub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    if (context.SecurityToken is JwtSecurityToken jwtToken)
                    {
                        var token = jwtToken.RawData;
                        var blacklistService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();

                        if (blacklistService.IsTokenBlacklistedAsync(token))
                        {
                            context.Fail("Токен удален");
                        }
                    }

                    return Task.CompletedTask;
                }
            };
        });
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
}
