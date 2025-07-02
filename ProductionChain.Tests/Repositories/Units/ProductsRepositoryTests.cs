using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Tests.Repositories.Units.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class ProductsRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<ProductsRepository>> _loggerMock;

    private readonly List<Product> _products;

    public ProductsRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<ProductsRepository>>();

        _products = new List<Product>
        {
            new()
            {
                Name = "Product1",
                Model = "Model1"
            },

            new()
            {
                Name = "Product2",
                Model = "Model2"
            }
        };
    }

    [Fact]
    public async Task GetProductsAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

        context.Products.AddRange(_products);
        await context.SaveChangesAsync();

        var mockRepository = new ProductsRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductsAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Products.Count);
    }

    [Fact]
    public async Task GetProductsAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = _dbContextFactory.CreateContext();

        context.Products.AddRange(_products);
        await context.SaveChangesAsync();

        var mockRepository = new ProductsRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetProductsAsync(new GetQueryParameters { Term = "Product2" });

        Assert.NotNull(result);
        Assert.Equal("Product2", result.Products.First().Name);
    }
}
