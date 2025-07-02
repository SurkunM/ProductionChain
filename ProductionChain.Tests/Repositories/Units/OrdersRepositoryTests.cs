using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Tests.Repositories.Units.DbContextFactory;

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

    [Fact]
    public async Task GetOrdersAsync_PaginationWorksCorrectly()//Тест на пагинацию
    {
        var context = _dbContextFactory.CreateContext();

        for (int i = 1; i <= 10; i++)
        {
            context.Orders.Add(new Order { Id = i, Customer = $"User{i}", Product = new Product { Name = $"Product{i}", Model = $"Model{i}" }, StageType = ProgressStatusType.Pending });
        }

        await context.SaveChangesAsync();

        var repo = new OrdersRepository(context, _loggerMock.Object);

        var firstPage = await repo.GetOrdersAsync(new GetQueryParameters { PageNumber = 1, PageSize = 5 });
        var secondPage = await repo.GetOrdersAsync(new GetQueryParameters { PageNumber = 2, PageSize = 5 });

        Assert.Equal(5, firstPage.Orders.Count);
        Assert.Equal(5, secondPage.Orders.Count);
        Assert.Equal(firstPage.Orders.Last().Id + 1, secondPage.Orders.First().Id);
    }

    [Fact]
    public async Task GetOrdersAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

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
        using var context = _dbContextFactory.CreateContext();

        context.Orders.AddRange(_orders);
        await context.SaveChangesAsync();

        var mockRepository = new OrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetOrdersAsync(new GetQueryParameters { Term = "Customer2" });

        Assert.NotNull(result);
        Assert.Equal("Customer2", result.Orders.First().Customer);
    }
}
