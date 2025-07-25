using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionOrdersRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionOrdersRepository>> _loggerMock;

    private readonly List<AssemblyProductionOrder> _productionOrdersList;

    public AssemblyProductionOrdersRepositoryTests()
    {
        _dbContextOptions = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<AssemblyProductionOrdersRepository>>();

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };

        _productionOrdersList = new List<AssemblyProductionOrder>
        {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
        };
    }

}
