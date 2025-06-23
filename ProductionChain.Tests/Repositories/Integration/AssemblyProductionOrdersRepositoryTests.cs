using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Repositories.Integration;

public class AssemblyProductionOrdersRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionOrdersRepository>> _loggerMock;

    private AssemblyProductionOrder _productionOrder;

    public AssemblyProductionOrdersRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<AssemblyProductionOrdersRepository>>();

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        _productionOrder = new AssemblyProductionOrder
        {
            Id = 1,
            Product = product,
            Order = new Order { Customer = "Customer1", Product = product, StageType = ProgressStatusType.InProgress },
            StatusType = ProgressStatusType.Pending,
            InProgressProductsCount = 100,
            CompletedProductsCount = 0,
            TotalProductsCount = 200
        };
    }

    [Fact]
    public async Task AddInProgressCount_ShouldIncreaseBy100()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);
        productionOrderRepository.AddInProgressCount(_productionOrder.Id, 100);

        var updatedProductionOrder = context.AssemblyProductionOrders.Find(_productionOrder.Id);

        Assert.NotNull(updatedProductionOrder);
        Assert.Equal(200, updatedProductionOrder.InProgressProductsCount);
    }

    [Fact]
    public async Task SubtractInProgressCount_ShouldDecreaseBy100()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);
        productionOrderRepository.SubtractInProgressCount(_productionOrder.Id, 100);

        var updatedProductionOrder = context.AssemblyProductionOrders.Find(_productionOrder.Id);

        Assert.NotNull(updatedProductionOrder);

        Assert.Equal(0, updatedProductionOrder.InProgressProductsCount);
    }

    [Fact]
    public async Task AddCompletedCount_ShouldIncreaseBy100()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        productionOrderRepository.AddCompletedCount(_productionOrder.Id, 100);

        var updatedProductionOrder = context.AssemblyProductionOrders.Find(_productionOrder.Id);

        Assert.NotNull(updatedProductionOrder);

        Assert.Equal(100, updatedProductionOrder.CompletedProductsCount);
    }

    [Theory]
    [InlineData(0, 0, ProgressStatusType.Pending)]
    [InlineData(0, 200, ProgressStatusType.Done)]
    [InlineData(100, 0, ProgressStatusType.InProgress)]
    public async Task UpdateProductionOrderStatus_ShouldReturnTrue(int inProgressCount, int completedCount, ProgressStatusType statusType)
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        _productionOrder.InProgressProductsCount = inProgressCount;
        _productionOrder.CompletedProductsCount = completedCount;

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        var result = productionOrderRepository.UpdateProductionOrderStatus(_productionOrder.Id);

        var updatedProductionOrder = context.AssemblyProductionOrders.Find(1);

        Assert.NotNull(updatedProductionOrder);

        Assert.Equal(updatedProductionOrder.StatusType, statusType);
    }

    [Fact]
    public async Task IsCompleted_ShouldReturnTrue()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        productionOrderRepository.AddCompletedCount(_productionOrder.Id, 200);

        var result = productionOrderRepository.IsCompleted(1);

        Assert.True(result);
    }

    [Fact]
    public async Task HasInProgressTasks_ShouldReturnFalse()
    {
        await using var context = new ProductionChainDbContext(_dbContextOptions);

        await context.AddAsync(_productionOrder);
        await context.SaveChangesAsync();

        var productionOrderRepository = new AssemblyProductionOrdersRepository(context, _loggerMock.Object);

        var result = productionOrderRepository.HasInProgressTasks(_productionOrder.Id);

        Assert.False(result);
    }
}
