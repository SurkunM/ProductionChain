using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.BusinessLogic.Handlers.Authorization;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
//[Authorize(Roles = "Admin")]
public class AuthorizationController : ControllerBase
{
    //private readonly AccountRegisterHandler _accountRegisterHandler;

    private readonly AccountAuthorizationHandler _accountAuthorizationHandlers;

    public AuthorizationController(AccountRegisterHandler registerHandler, AccountAuthorizationHandler authorizationHandlers)
    {
        //_accountRegisterHandler = registerHandler ?? throw new ArgumentNullException(nameof(registerHandler));
        _accountAuthorizationHandlers = authorizationHandlers ?? throw new ArgumentNullException(nameof(authorizationHandlers));
    }

    [HttpPost]
    public async Task<IActionResult> Register(AuthAccountRegisterRequest accountRegisterRequest)
    {
        await _accountAuthorizationHandlers.HandleAsync(accountRegisterRequest);

        return Ok();
    }
}
