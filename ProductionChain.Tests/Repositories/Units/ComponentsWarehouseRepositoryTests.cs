using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

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


}
