using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Tests.Repositories.Units;

public class OrdersRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<OrdersRepository>> _loggerMock;

    private readonly List<Order> _orders;

    public OrdersRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

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

    [Fact]
    public async Task GetOrdersAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.Orders.AddRange(_orders);
        await context.SaveChangesAsync();

        var mockRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetOrdersAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Orders.Count);
    }

    [Fact]
    public async Task GetOrdersAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.Orders.AddRange(_orders);
        await context.SaveChangesAsync();

        var mockRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetOrdersAsync(new GetQueryParameters { Term = "Customer2" });

        Assert.NotNull(result);
        Assert.Equal("Customer2", result.Orders.First().Customer);
    }
}
