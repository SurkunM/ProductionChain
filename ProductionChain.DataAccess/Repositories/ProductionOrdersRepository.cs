using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionOrdersRepository : BaseEfRepository<ProductionOrders>, IProductionOrdersRepository
{
    private readonly ILogger<ProductionOrdersRepository> _logger;

    public ProductionOrdersRepository(ProductionChainDbContext dbContext, ILogger<ProductionOrdersRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}