using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Units.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionOrdersRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<AssemblyProductionOrdersRepository>> _loggerMock;

    private readonly List<AssemblyProductionOrder> _productionOrdersList;

    public AssemblyProductionOrdersRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<AssemblyProductionOrdersRepository>>();

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };

        _productionOrdersList = new List<AssemblyProductionOrder>
        {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
        };
    }

    [Fact]
    public async Task GetProductionOrdersAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

        var dbSet = context.Set<AssemblyProductionOrder>();

        dbSet.AddRange(_productionOrdersList);
        context.SaveChanges();

        var mockRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductionOrdersAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.ProductionOrders.Count);
    }

    [Fact]
    public async Task GetProductionOrders_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = _dbContextFactory.CreateContext();

        var dbSet = context.Set<AssemblyProductionOrder>();

        dbSet.AddRange(_productionOrdersList);
        context.SaveChanges();

        var mockRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductionOrdersAsync(new GetQueryParameters { Term = "Product2" });

        Assert.NotNull(result);
        Assert.Equal("Product2", result.ProductionOrders.First().ProductName);
    }
}
