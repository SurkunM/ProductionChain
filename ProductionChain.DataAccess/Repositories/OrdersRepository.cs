using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.DataAccess.Repositories;

public class OrdersRepository : BaseEfRepository<Order>, IOrdersRepository
{
    private readonly ILogger<OrdersRepository> _logger;

    public OrdersRepository(ProductionChainDbContext dbContext, ILogger<OrdersRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
