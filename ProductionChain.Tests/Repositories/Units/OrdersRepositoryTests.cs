using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class OrdersRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<OrdersRepository>> _loggerMock;

    private readonly List<Order> _orders;

    public OrdersRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<OrdersRepository>>();

        _orders = new List<Order>
        {
            new()
            {
                Customer = "Customer1",
                Product = new Product { Name  = "Product1", Model = "Model1"},
                StageType = ProgressStatusType.Pending
            },

            new()
            {
                Customer = "Customer1",
                Product = new Product { Name  = "Product1", Model = "Model1"},
                StageType = ProgressStatusType.Pending
            },

            new()
            {
                Customer = "Customer2",
                Product = new Product { Name  = "Product2", Model = "Model2"},
                StageType = ProgressStatusType.Pending
            }
        };
    }
}
