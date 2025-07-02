using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;

namespace ProductionChain.Tests.Repositories.Units.DbContextFactory;

public class ProductionChainDbContextFactory : IDisposable
{
    private readonly DbContextOptions<ProductionChainDbContext> _options;

    public ProductionChainDbContextFactory()
    {
        _options = new DbContextOptionsBuilder<ProductionChainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    public ProductionChainDbContext CreateContext()
    {
        return new ProductionChainDbContext(_options);
    }

    public void Dispose()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
    }
}
