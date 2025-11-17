using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Identity;
using ProductionChain.Contracts.Dto.Requests;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthorizationController : ControllerBase
{
    private readonly AccountAuthorizationHandler _accountAuthorizationHandlers;

    public AuthorizationController(AccountAuthorizationHandler authorizationHandlers)
    {
        _accountAuthorizationHandlers = authorizationHandlers ?? throw new ArgumentNullException(nameof(authorizationHandlers));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AccountRegister(AuthAccountRegisterRequest accountRegisterRequest)
    {
        await _accountAuthorizationHandlers.HandleAsync(accountRegisterRequest);

        return Ok();
    }
}
