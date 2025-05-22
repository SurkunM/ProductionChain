using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyWarehouseRepository : BaseEfRepository<ProductionAssemblyWarehouse>, IProductionAssemblyWarehouseRepository
{
    private readonly ILogger<ProductionAssemblyWarehouseRepository> _logger;

    public ProductionAssemblyWarehouseRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyWarehouseRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
