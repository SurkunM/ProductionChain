using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

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
}
