using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Repositories.Units;

public class ProductionHistoryRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<ProductionHistoryRepository>> _loggerMock;

    private readonly List<ProductionHistory> _histories;

    public ProductionHistoryRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<ProductionHistoryRepository>>();

        _histories = new List<ProductionHistory>
        {
            new()
            {
                Employee = "Employee1",
                Product = "Product1",
                ProductsCount = 10
            },

            new()
            {
                Employee = "Employee2",
                Product = "Product2",
                ProductsCount = 10
            }
        };
    }

    [Fact]
    public async Task GetProductionHistoriesAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.ProductionHistory.AddRange(_histories);
        await context.SaveChangesAsync();

        var mockRepository = new ProductionHistoryRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductionHistoriesAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Histories.Count);
    }

    [Fact]
    public async Task GetProductionHistoriesAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.ProductionHistory.AddRange(_histories);
        await context.SaveChangesAsync();

        var mockRepository = new ProductionHistoryRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductionHistoriesAsync(new GetQueryParameters { Term = "Employee2" });

        Assert.NotNull(result);
        Assert.Equal("Employee2", result.Histories.First().Employee);
    }
}
