using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Tests.Repositories.Integration.DbContextFactory;

namespace ProductionChain.Tests.Repositories.Integration;

public class EmployeesRepositoryTests
{
    private readonly ProductionChainDbContextFactory _dbContextFactory;

    private readonly Mock<ILogger<EmployeesRepository>> _loggerMock;

    private readonly List<Employee> _employees;

    public EmployeesRepositoryTests()
    {
        _dbContextFactory = new ProductionChainDbContextFactory();

        _loggerMock = new Mock<ILogger<EmployeesRepository>>();

        _employees = new List<Employee>
        {
            new()
            {
                FirstName = "FirstName1",
                LastName = "LastName1",
                MiddleName = "MiddleName1",
                Position = EmployeePositionType.AssemblyREA,
                Status = EmployeeStatusType.Available
            },

            new()
            {
                FirstName = "FirstName2",
                LastName = "LastName2",
                MiddleName = "MiddleName2",
                Position = EmployeePositionType.AssemblyREA,
                Status = EmployeeStatusType.Available
            },

            new()
            {
                FirstName = "FirstName3",
                LastName = "LastName3",
                MiddleName = "MiddleName3",
                Position = EmployeePositionType.AssemblyREA,
                Status = EmployeeStatusType.Available
            }
        };
    }

    [Fact]
    public async Task GetEmployeesAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        using var context = _dbContextFactory.CreateContext();

        await context.Employees.AddRangeAsync(_employees);
        await context.SaveChangesAsync();

        var mockRepository = new EmployeesRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetEmployeesAsync(new GetQueryParameters());

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Employees.Count);
    }

    [Fact]
    public async Task GetEmployeesAsync_FilterByTerm_ReturnsFilteredResults()
    {
        using var context = _dbContextFactory.CreateContext();

        await context.Employees.AddRangeAsync(_employees);
        await context.SaveChangesAsync();

        var mockRepository = new EmployeesRepository(context, _loggerMock.Object);

        var result = await mockRepository.GetEmployeesAsync(new GetQueryParameters { Term = "FirstName2" });

        Assert.NotNull(result);
        Assert.Equal("FirstName2", result.Employees.First().FirstName);
    }
}
