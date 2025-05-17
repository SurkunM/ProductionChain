using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionHistoryRepository : BaseEfRepository<ProductionHistory>, IProductionHistoryRepository
{
    private readonly ILogger<ProductionStagesRepository> _logger;

    public ProductionHistoryRepository(ProductionChainDbContext dbContext, ILogger<ProductionStagesRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}