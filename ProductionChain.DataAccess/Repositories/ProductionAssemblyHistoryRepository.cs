using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyHistoryRepository : BaseEfRepository<ProductionAssemblyHistory>, IProductionAssemblyHistoryRepository
{
    private readonly ILogger<ProductionAssemblyHistoryRepository> _logger;

    public ProductionAssemblyHistoryRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyHistoryRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}