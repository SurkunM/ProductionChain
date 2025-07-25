using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class ProductionHistoryRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<ProductionHistoryRepository>> _loggerMock;

    private readonly List<ProductionHistory> _histories;

    public ProductionHistoryRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

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
}
