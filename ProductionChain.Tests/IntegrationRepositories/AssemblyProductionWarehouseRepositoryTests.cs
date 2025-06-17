using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.IntegrationRepositories;

public class AssemblyProductionWarehouseRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionWarehouseRepository>> _loggerMock;

    private AssemblyProductionWarehouse _assemblyWarehouse;

    public AssemblyProductionWarehouseRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<AssemblyProductionWarehouseRepository>>();

        _assemblyWarehouse = new AssemblyProductionWarehouse
        {
            Product = new Product { Name = "Product1", Model = "Model1" },
            ProductsCount = 100
        };
    }

    [Fact]
    public async Task AddWarehouseItems()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_assemblyWarehouse);
        await context.SaveChangesAsync();

        var assemblyWarehouseRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        assemblyWarehouseRepository.AddWarehouseItems(_assemblyWarehouse.ProductId, 100);

        var updatedAssemblyWarehouse = context.AssemblyProductionWarehouse.Find(1);

        Assert.NotNull(updatedAssemblyWarehouse);

        Assert.Equal(200, updatedAssemblyWarehouse.ProductsCount);
    }

    [Fact]
    public async Task SubtractWarehouseItems()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_assemblyWarehouse);
        await context.SaveChangesAsync();

        var assemblyWarehouseRepository = new AssemblyProductionWarehouseRepository(context, _loggerMock.Object);

        assemblyWarehouseRepository.SubtractWarehouseItems(_assemblyWarehouse.ProductId, 100);

        var updatedAssemblyWarehouse = context.AssemblyProductionWarehouse.Find(1);

        Assert.NotNull(updatedAssemblyWarehouse);

        Assert.Equal(0, updatedAssemblyWarehouse.ProductsCount);
    }
}
