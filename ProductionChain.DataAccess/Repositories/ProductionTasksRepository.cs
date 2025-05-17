using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionTasksRepository : BaseEfRepository<ProductionTask>, IProductionTasksRepository
{
    private readonly ILogger<ProductionTasksRepository> _logger;

    public ProductionTasksRepository(ProductionChainDbContext dbContext, ILogger<ProductionTasksRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
