using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Tests.IntegrationRepositories;

public class OrdersRepositoryTests : IDisposable
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<OrdersRepository>> _loggerMock;

    public OrdersRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<OrdersRepository>>();
    }

    [Theory]
    [InlineData(ProgressStatusType.Pending, true)]
    [InlineData(ProgressStatusType.Done, false)]
    [InlineData(ProgressStatusType.InProgress, false)]
    public async Task ShouldReturnTrueForOrderIsPending(ProgressStatusType status, bool expected)
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Id = 1,
            Customer = "Customer1",
            Product = product,
            StageType = status,
            OrderedProductsCount = 100
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var ordersRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = ordersRepository.IsOrderPending(1);

        Assert.Equal(expected, ordersRepository.IsOrderPending(1));
    }

    [Fact]
    public async Task ShouldUpdateAvailableProductsCountIncreaseBy100()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Id = 1,
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress,
            OrderedProductsCount = 100,
            AvailableProductsCount = 100
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var updatedOrder = context.Orders.Find(1);

        Assert.NotNull(updatedOrder);

        updatedOrder.AvailableProductsCount += 100;

        Assert.Equal(200, updatedOrder.AvailableProductsCount);
    }

    public void Dispose()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);
        context.Database.EnsureDeleted();
    }
}
