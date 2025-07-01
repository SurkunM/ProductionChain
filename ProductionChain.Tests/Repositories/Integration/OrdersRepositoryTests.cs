using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Tests.Repositories.Integration;

public class OrdersRepositoryTests
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
    public async Task OrderIsPending_ShouldReturnTrue(ProgressStatusType status, bool expected)
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        var product = GetProduct("Product1", "Model1");
        var order = GetOrder(product, "Customer1", status);

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var ordersRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = ordersRepository.IsOrderPending(1);

        Assert.Equal(expected, ordersRepository.IsOrderPending(1));
    }

    [Fact]
    public async Task SetAvailableProductsCount_ShouldIncreaseBy100()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        var product = GetProduct("Product1", "Model1");
        var order = GetOrder(product, "Customer1", ProgressStatusType.InProgress);

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var ordersRepository = new OrdersRepository(context, _loggerMock.Object);

        ordersRepository.SetAvailableProductsCount(order.Id, 100);

        var updatedOrder = context.Orders.Find(1);

        Assert.NotNull(updatedOrder);

        Assert.Equal(100, updatedOrder.AvailableProductsCount);
    }

    private static Product GetProduct(string name, string model)
    {
        return new Product
        {
            Name = name,
            Model = model
        };
    }

    private static Order GetOrder(Product product, string customer, ProgressStatusType status)
    {
        return new Order
        {
            Id = 1,
            Customer = "Customer1",
            Product = product,
            StageType = status,
            OrderedProductsCount = 100,
            AvailableProductsCount = 0
        };
    }
}
