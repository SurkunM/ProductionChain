using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ComponentsWarehouseRepository : BaseEfRepository<ComponentsWarehouse>, IComponentsWarehouseRepository
{
    private readonly ILogger<ComponentsWarehouseRepository> _logger;

    public ComponentsWarehouseRepository(ProductionChainDbContext dbContext, ILogger<ComponentsWarehouseRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
