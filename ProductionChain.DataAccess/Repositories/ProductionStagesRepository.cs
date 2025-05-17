using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionStagesRepository : BaseEfRepository<ProductionStage>, IProductionStagesRepository
{
    private readonly ILogger<ProductionStagesRepository> _logger;

    public ProductionStagesRepository(ProductionChainDbContext dbContext, ILogger<ProductionStagesRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
