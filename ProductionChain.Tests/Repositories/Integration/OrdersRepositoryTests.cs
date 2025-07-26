using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Integration;

public class OrdersRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<OrdersRepository>> _loggerMock;

    private readonly List<Order> _orders;

    private readonly Product _product;

    public OrdersRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<OrdersRepository>>();

        _product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        _orders = new List<Order>
        {
            new()
            {
                Customer = "Customer1",
                Product = _product,
                StageType = ProgressStatusType.Pending
            },

            new()
            {
                Customer = "Customer1",
                Product = _product,
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
        using var context = _dbContextFactory.CreateContext();

        await context.Orders.AddRangeAsync(_orders);
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
        using var context = _dbContextFactory.CreateContext();

        await context.Orders.AddRangeAsync(_orders);
        await context.SaveChangesAsync();

        var mockRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetOrdersAsync(new GetQueryParameters { Term = "Customer2" });

        Assert.NotNull(result);
        Assert.Equal("Customer2", result.Orders.First().Customer);
    }

    [Theory]
    [InlineData(ProgressStatusType.Pending, true)]
    [InlineData(ProgressStatusType.Done, false)]
    [InlineData(ProgressStatusType.InProgress, false)]
    public async Task OrderIsPending_ShouldReturnTrue(ProgressStatusType status, bool expected)
    {
        await using var context = _dbContextFactory.CreateContext();

        var order = GetOrder(_product, "Customer1", status);

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var ordersRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = ordersRepository.IsOrderPending(1);

        Assert.Equal(expected, ordersRepository.IsOrderPending(1));
    }

    [Fact]
    public async Task SetAvailableProductsCount_ShouldIncreaseBy100()
    {
        await using var context = _dbContextFactory.CreateContext();

        var order = GetOrder(_product, "Customer1", ProgressStatusType.InProgress);

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var ordersRepository = new OrdersRepository(context, _loggerMock.Object);

        ordersRepository.SetAvailableProductsCount(order.Id, 100);

        var updatedOrder = context.Orders.Find(1);

        Assert.NotNull(updatedOrder);

        Assert.Equal(100, updatedOrder.AvailableProductsCount);
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
