using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Tests.Handlers;

public class AccountAuthorizationHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly Mock<UserManager<Account>> _userManager;

    private readonly AccountAuthorizationHandler _accountAuthorizationHandler;

    private readonly Mock<ILogger<AccountAuthorizationHandler>> _loggerMock;

    private readonly Account _account;

    private readonly Employee _employee;

    private readonly AuthAccountRegisterRequest _authAccountRegisterRequest;

    public AccountAuthorizationHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AccountAuthorizationHandler>>();

        var store = new Mock<IUserStore<Account>>();
        _userManager = new Mock<UserManager<Account>>(store.Object, null, null, null, null, null, null, null, null);

        _accountAuthorizationHandler = new AccountAuthorizationHandler(_userManager.Object, _uowMock.Object, _loggerMock.Object);

        _employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        _account = new Account
        {
            EmployeeId = _employee.Id,
            Employee = _employee,
            UserName = "User1"
        };

        _authAccountRegisterRequest = new AuthAccountRegisterRequest
        {
            EmployeeId = _employee.Id,
            Login = "User1",
            Password = "User1Password",
            Role = "Employee"
        };
    }

    [Fact]
    public async Task Should_Successfully_AuthorizationAllSteps_And_SaveChanges()
    {
        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId)).ReturnsAsync(_employee);
        employeesRepositoryMock.Setup(r => r.AddAccount(_authAccountRegisterRequest.EmployeeId, _account)).Returns(true);

        _userManager.Setup(um => um.CreateAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Password)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(um => um.AddToRoleAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Role)).ReturnsAsync(IdentityResult.Success);

        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);

        await _accountAuthorizationHandler.HandleAsync(_authAccountRegisterRequest);

        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId), Times.Once);
        employeesRepositoryMock.Verify(r => r.AddAccount(_authAccountRegisterRequest.EmployeeId, It.IsAny<Account>()), Times.Once);

        _userManager.Verify(um => um.CreateAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Password), Times.Once);
        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Role), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_NotFoundException_When_GetByIdAsync_Return_IsNull()
    {
        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId)).ReturnsAsync((Employee)null!);

        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _accountAuthorizationHandler.HandleAsync(_authAccountRegisterRequest));

        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId), Times.Once);
        employeesRepositoryMock.Verify(r => r.AddAccount(_authAccountRegisterRequest.EmployeeId, _account), Times.Never);

        _userManager.Verify(um => um.AddToRoleAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Role), Times.Never);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_AccountNotCreatedException_When_CreateAsync_Return_NotSucceeded()
    {
        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId)).ReturnsAsync(_employee);

        _userManager.Setup(um => um.CreateAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Password)).ReturnsAsync(IdentityResult.Failed());

        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);

        await Assert.ThrowsAsync<AccountNotCreatedException>(() => _accountAuthorizationHandler.HandleAsync(_authAccountRegisterRequest));

        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_authAccountRegisterRequest.EmployeeId), Times.Once);
        employeesRepositoryMock.Verify(r => r.AddAccount(_authAccountRegisterRequest.EmployeeId, It.IsAny<Account>()), Times.Never);

        _userManager.Verify(um => um.CreateAsync(It.IsAny<Account>(), _authAccountRegisterRequest.Password), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }
}
