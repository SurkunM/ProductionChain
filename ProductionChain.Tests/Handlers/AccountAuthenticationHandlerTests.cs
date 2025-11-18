using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProductionChain.BusinessLogic.Handlers.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Tests.Handlers;

public class AccountAuthenticationHandlerTests
{
    private readonly Mock<UserManager<Account>> _userManager;

    private readonly SignInManager<Account> _signInManager;

    private readonly Mock<IJwtGenerationService> _jwtGenerationService;

    private readonly AccountAuthenticationHandler _accountAuthenticationHandler;

    private readonly Account _account;

    private readonly AccountLoginRequest _authenticationLoginRequest;

    public AccountAuthenticationHandlerTests()
    {
        var store = new Mock<IUserStore<Account>>();
        _userManager = new Mock<UserManager<Account>>(store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();

        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Account>>();

        _signInManager = new SignInManager<Account>(
            _userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null
        );

        _jwtGenerationService = new Mock<IJwtGenerationService>();

        _accountAuthenticationHandler = new AccountAuthenticationHandler(_userManager.Object, _signInManager, _jwtGenerationService.Object);

        _account = new Account
        {
            EmployeeId = 1,
            Employee = new Employee
            {
                Id = 1,
                FirstName = "Employee1_FirstName",
                LastName = "Employee1_LastName",
                MiddleName = "Employee1_MiddleName",
                Position = EmployeePositionType.AssemblyREA,
                Status = EmployeeStatusType.Busy
            }
        };

        _authenticationLoginRequest = new AccountLoginRequest
        {
            UserLogin = "User1",
            Password = "User1Password"
        };
    }

    [Fact]
    public async Task Should_Successfully_AuthenticationAllSteps_And_Return_AuthenticationLoginResponse()
    {
        _userManager.Setup(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin)).ReturnsAsync(_account);
        _userManager.Setup(um => um.GetRolesAsync(_account)).ReturnsAsync(new List<string> { "User" });

        _userManager.Setup(sm => sm.CheckPasswordAsync(_account, _authenticationLoginRequest.Password)).ReturnsAsync(true);

        _jwtGenerationService.Setup(jwtGS => jwtGS.GenerateTokenAsync(_account)).ReturnsAsync("token");

        var result = await _accountAuthenticationHandler.HandleAsync(_authenticationLoginRequest);

        Assert.NotNull(result);
        Assert.Equal("token", result.Token);
        Assert.Equal(1, result.UserData.UserId);
        Assert.Equal("Employee1_LastName E. E.", result.UserData.UserName);
        Assert.Equal(new[] { "User" }, result.UserData.Roles);

        _userManager.Verify(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(_account), Times.Once);
        _userManager.Verify(sm => sm.CheckPasswordAsync(_account, _authenticationLoginRequest.Password), Times.Once);

        _jwtGenerationService.Verify(jwtGS => jwtGS.GenerateTokenAsync(_account), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_FindByNameAsync_Return_IsNull()
    {
        _userManager.Setup(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin)).ReturnsAsync((Account)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _accountAuthenticationHandler.HandleAsync(_authenticationLoginRequest));

        _userManager.Verify(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin), Times.Once);
        _userManager.Verify(sm => sm.CheckPasswordAsync(_account, _authenticationLoginRequest.Password), Times.Never);
        _userManager.Verify(um => um.GetRolesAsync(_account), Times.Never);

        _jwtGenerationService.Verify(jwtGS => jwtGS.GenerateTokenAsync(_account), Times.Never);
    }

    [Fact]
    public async Task Should_UnauthorizedAccessException_When_CheckPasswordAsync_Return_NotAllowed()
    {
        _userManager.Setup(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin)).ReturnsAsync(_account);
        _userManager.Setup(sm => sm.CheckPasswordAsync(_account, _authenticationLoginRequest.Password)).ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _accountAuthenticationHandler.HandleAsync(_authenticationLoginRequest));

        _userManager.Verify(um => um.FindByNameAsync(_authenticationLoginRequest.UserLogin), Times.Once);
        _userManager.Verify(sm => sm.CheckPasswordAsync(_account, _authenticationLoginRequest.Password), Times.Once);
        _userManager.Verify(um => um.GetRolesAsync(_account), Times.Never);

        _jwtGenerationService.Verify(jwtGS => jwtGS.GenerateTokenAsync(_account), Times.Never);
    }
}
