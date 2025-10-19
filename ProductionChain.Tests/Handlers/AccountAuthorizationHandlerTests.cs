using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.Authorization;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Tests.Handlers;

public class AccountAuthorizationHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly Mock<UserManager<Account>> _userManager;

    private readonly AccountAuthorizationHandler _accountAuthorizationHandler;

    private readonly Mock<ILogger<AccountAuthorizationHandler>> _loggerMock;

    public AccountAuthorizationHandlerTests()
    {
        _userManager = new Mock<UserManager<Account>>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AccountAuthorizationHandler>>();

        _accountAuthorizationHandler = new AccountAuthorizationHandler(_userManager.Object, _uowMock.Object, _loggerMock.Object);
    }
}
