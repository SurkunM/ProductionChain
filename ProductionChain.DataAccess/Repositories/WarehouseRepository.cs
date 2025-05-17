using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.DataAccess.Repositories;

public class WarehouseRepository : BaseEfRepository<Warehouse>, IWarehouseRepository
{
    private readonly ILogger<WarehouseRepository> _logger;

    public WarehouseRepository(ProductionChainDbContext dbContext, ILogger<WarehouseRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
