using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.DataAccess;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Repositories.Integration;

public class AssemblyProductionTasksRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionTasksRepositoryTests>> _loggerMock;

    private AssemblyProductionTask _task;

    public AssemblyProductionTasksRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<AssemblyProductionTasksRepositoryTests>>();

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var productionOrder = new AssemblyProductionOrder
        {
            Id = 1,
            Product = product,
            Order = new Order { Customer = "Customer1", Product = product, StageType = ProgressStatusType.InProgress },
            StatusType = ProgressStatusType.Pending,
            InProgressProductsCount = 100,
            CompletedProductsCount = 0,
            TotalProductsCount = 100
        };

        _task = new AssemblyProductionTask
        {
            ProductionOrder = productionOrder,
            Product = product,
            Employee = new Employee { FirstName = "Employee1", LastName = "Employee1", Position = EmployeePositionType.AssemblyREA, Status = EmployeeStatusType.Available }
        };
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnCorrectData()
    {

    }
}
