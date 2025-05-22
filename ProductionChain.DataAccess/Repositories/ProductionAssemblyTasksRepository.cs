using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyTasksRepository : BaseEfRepository<ProductionAssemblyTask>, IProductionAssemblyTasksRepository
{
    private readonly ILogger<ProductionAssemblyTasksRepository> _logger;

    public ProductionAssemblyTasksRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyTasksRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
