using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductsRepository : BaseEfRepository<Product>, IProductsRepository
{
    private readonly ILogger<ProductsRepository> _logger;

    public ProductsRepository(ProductionChainDbContext dbContext, ILogger<ProductsRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
