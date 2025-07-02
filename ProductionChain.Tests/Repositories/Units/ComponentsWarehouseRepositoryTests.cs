using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Units.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class ComponentsWarehouseRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<ComponentsWarehouseRepository>> _loggerMock;

    private readonly List<ComponentsWarehouse> _componentsWarehouseItems;

    public ComponentsWarehouseRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<ComponentsWarehouseRepository>>();

        _componentsWarehouseItems = new List<ComponentsWarehouse>
        {
            new()
            {
                Product = new Product { Name = "Product1", Model = "Model1"},
                Type = ComponentType.Enclosure,
                ComponentsCount = 100
            },

            new()
            {
                Product = new Product { Name = "Product1", Model = "Model1"},
                Type = ComponentType.CircuitBoard,
                ComponentsCount = 100
            },

            new()
            {
                Product = new Product { Name = "Product2", Model = "Model2"},
                Type = ComponentType.Heatsink,
                ComponentsCount = 100
            }
        };
    }

    [Fact]
    public async Task GetComponentsAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

        context.ComponentsWarehouse.AddRange(_componentsWarehouseItems);
        await context.SaveChangesAsync();

        var mockRepository = new ComponentsWarehouseRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetComponentsAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.ComponentsWarehouseItems.Count);
    }

    [Fact]
    public async Task GetComponentsAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = _dbContextFactory.CreateContext();

        context.ComponentsWarehouse.AddRange(_componentsWarehouseItems);
        await context.SaveChangesAsync();

        var mockRepository = new ComponentsWarehouseRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetComponentsAsync(new GetQueryParameters { Term = "Product2" });

        Assert.NotNull(result);
        Assert.Equal("Product2", result.ComponentsWarehouseItems.First().ProductName);
    }
}
