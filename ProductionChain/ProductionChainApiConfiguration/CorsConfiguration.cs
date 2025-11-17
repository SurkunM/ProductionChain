namespace ProductionChain.ProductionChainApiConfiguration;

public static class CorsConfiguration
{
    public static void ConfigureProductionChainCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("VueFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:8080", "https://localhost:8080")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }
}
