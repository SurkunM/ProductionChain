using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.IntegrationRepositories;

public class ComponentsWarehouseRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<ComponentsWarehouseRepository>> _loggerMock;

    public ComponentsWarehouseRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<ComponentsWarehouseRepository>>();
    }

    [Fact]
    public async Task TakeComponentsByProductId_ShouldDecreaseComponentsBy50()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        var product = new Product()
        {
            Name = "Product1",
            Model = "Model1",
        };
        var componentsWarehouseItem1 = CreateComponentsWarehouseItem(product, ComponentType.CircuitBoard, 100);
        var componentsWarehouseItem2 = CreateComponentsWarehouseItem(product, ComponentType.DiodeBoard, 100);
        var componentsWarehouseItem3 = CreateComponentsWarehouseItem(product, ComponentType.Heatsink, 100);
        var componentsWarehouseItem4 = CreateComponentsWarehouseItem(product, ComponentType.Enclosure, 100);


        await context.AddRangeAsync(componentsWarehouseItem1, componentsWarehouseItem2, componentsWarehouseItem3, componentsWarehouseItem4);
        await context.SaveChangesAsync();

        var componentsWarehouseRepository = new ComponentsWarehouseRepository(context, _loggerMock.Object);

        componentsWarehouseRepository.TakeComponentsByProductId(1, 50);

        var updatedComponentsWarehouse = context.ComponentsWarehouse.Find(1);

        Assert.NotNull(updatedComponentsWarehouse);

        Assert.Equal(50, updatedComponentsWarehouse.ComponentsCount);
    }

    private static ComponentsWarehouse CreateComponentsWarehouseItem(Product product, ComponentType componentType, int count)
    {
        return new ComponentsWarehouse()
        {
            Product = product,
            Type = componentType,
            ComponentsCount = count
        };
    }
}
