using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionWarehouseRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<AssemblyProductionWarehouseRepository>> _loggerMock;

    private readonly List<AssemblyProductionWarehouse> _assemblyWarehouseItems;

    public AssemblyProductionWarehouseRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

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
}
