using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionWarehouseRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionWarehouseRepository>> _loggerMock;

    private List<AssemblyProductionWarehouse> _assemblyWarehouseItems;

    public AssemblyProductionWarehouseRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<AssemblyProductionWarehouseRepository>>();

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
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.AssemblyProductionWarehouse.AddRange(_assemblyWarehouseItems);
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
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.AssemblyProductionWarehouse.AddRange(_assemblyWarehouseItems);
        await context.SaveChangesAsync();

        var mockRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetAssemblyWarehouseItemsAsync(new GetQueryParameters { Term = "Product2" });

        Assert.NotNull(result);
        Assert.Equal("Product2", result.AssemblyWarehouseItems.First().Name);
    }
}
