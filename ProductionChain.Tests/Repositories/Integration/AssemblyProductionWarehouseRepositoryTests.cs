using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Integration;

public class AssemblyProductionWarehouseRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<AssemblyProductionWarehouseRepository>> _loggerMock;

    private readonly AssemblyProductionWarehouse _assemblyWarehouse;

    private readonly List<AssemblyProductionWarehouse> _assemblyWarehouseItems;

    public AssemblyProductionWarehouseRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<AssemblyProductionWarehouseRepository>>();

        _assemblyWarehouse = new AssemblyProductionWarehouse
        {
            Product = new Product { Name = "Product1", Model = "Model1" },
            ProductsCount = 100
        };

        _assemblyWarehouseItems =
        [
            new()
            {
                Product = new Product { Name = "Product1", Model = "Model1" },
                ProductsCount = 100
            },

            new()
            {
                Product = new Product { Name = "Product2", Model = "Model2" },
                ProductsCount = 100
            },

            new()
            {
                Product = new Product { Name = "Product3", Model = "Model3" },
                ProductsCount = 100
            }
        ];
    }

    [Fact]
    public async Task GetAssemblyWarehouseItemsAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

        await context.AssemblyProductionWarehouse.AddRangeAsync(_assemblyWarehouseItems);
        await context.SaveChangesAsync();

        var mockRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetAssemblyWarehouseItemsAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.AssemblyWarehouseItems.Count);
    }

    [Fact]
    public async Task GetAssemblyWarehouseItemsAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = _dbContextFactory.CreateContext();

        await context.AssemblyProductionWarehouse.AddRangeAsync(_assemblyWarehouseItems);
        await context.SaveChangesAsync();

        var mockRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetAssemblyWarehouseItemsAsync(new GetQueryParameters { Term = "Product2" });

        Assert.NotNull(result);
        Assert.Equal("Product2", result.AssemblyWarehouseItems.First().Name);
    }

    [Fact]
    public async Task AddWarehouseItems_ShouldIncreaseItemsBy100()
    {
        await using var context = _dbContextFactory.CreateContext();

        await context.AssemblyProductionWarehouse.AddAsync(_assemblyWarehouse);
        await context.SaveChangesAsync();

        var assemblyWarehouseRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        assemblyWarehouseRepository.AddWarehouseItems(_assemblyWarehouse.ProductId, 100);

        var updatedAssemblyWarehouse = context.AssemblyProductionWarehouse.Find(1);

        Assert.NotNull(updatedAssemblyWarehouse);

        Assert.Equal(200, updatedAssemblyWarehouse.ProductsCount);
    }

    [Fact]
    public async Task SubtractWarehouseItems_ShouldDecreaseItemsBy100()
    {
        await using var context = _dbContextFactory.CreateContext();

        await context.AssemblyProductionWarehouse.AddAsync(_assemblyWarehouse);
        await context.SaveChangesAsync();

        var assemblyWarehouseRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        assemblyWarehouseRepository.SubtractWarehouseItems(_assemblyWarehouse.ProductId, 100);

        var updatedAssemblyWarehouse = context.AssemblyProductionWarehouse.Find(1);

        Assert.NotNull(updatedAssemblyWarehouse);

        Assert.Equal(0, updatedAssemblyWarehouse.ProductsCount);
    }
}
