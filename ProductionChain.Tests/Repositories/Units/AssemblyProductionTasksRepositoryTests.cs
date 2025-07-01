using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionTasksRepositoryTests
{
    private readonly DbContextOptions<ProductionChainDbContext> _dbContextOptions;

    private readonly Mock<ILogger<AssemblyProductionTasksRepository>> _loggerMock;

    private List<AssemblyProductionTask> _tasks;

    public AssemblyProductionTasksRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProductionChainDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _loggerMock = new Mock<ILogger<AssemblyProductionTasksRepository>>();

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

        _tasks = new List<AssemblyProductionTask>
        {
            new()
            {
                ProductionOrder = productionOrder,
                Product = product,
                Employee = new Employee { FirstName = "FirstName1", LastName = "LastName1", Position = EmployeePositionType.AssemblyREA, Status = EmployeeStatusType.Available }
            },

            new()
            {
                ProductionOrder = productionOrder,
                Product = product,
                Employee = new Employee { FirstName = "FirstName2", LastName = "LastName2", Position = EmployeePositionType.AssemblyREA, Status = EmployeeStatusType.Available }
            }
        };
    }

    [Fact]
    public async Task GetTasksAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.AssemblyProductionTasks.AddRange(_tasks);
        await context.SaveChangesAsync();

        var mockRepository = new AssemblyProductionTasksRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetTasksAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Tasks.Count);//TODO: Придумать проверку по лучше
    }

    [Fact]
    public async Task GetTasksAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = new ProductionChainDbContext(_dbContextOptions);

        context.AssemblyProductionTasks.AddRange(_tasks);
        await context.SaveChangesAsync();

        var mockRepository = new AssemblyProductionTasksRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetTasksAsync(new GetQueryParameters { Term = "FirstName2" });

        Assert.NotNull(result);
        Assert.Equal("FirstName2", result.Tasks.First().EmployeeFirstName);
    }
}
