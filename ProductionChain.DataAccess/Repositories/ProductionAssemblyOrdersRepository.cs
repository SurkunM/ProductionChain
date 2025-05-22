using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyOrdersRepository : BaseEfRepository<ProductionAssemblyOrders>, IProductionAssemblyOrdersRepository
{
    private readonly ILogger<ProductionAssemblyOrdersRepository> _logger;

    public ProductionAssemblyOrdersRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyOrdersRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}