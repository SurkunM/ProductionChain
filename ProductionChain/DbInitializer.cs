using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;

namespace ProductionChain;

public class DbInitializer
{
    private readonly ProductionChainDbContext _dbContext;

    public DbInitializer(ProductionChainDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Initialize()
    {
        _dbContext.Database.Migrate();
    }
}
