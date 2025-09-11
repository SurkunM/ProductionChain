using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.BusinessLogic.Handlers.Authorization;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api[controller]/[action]")]
[Authorize(Roles = "Admin")]
public class AuthorizationController : ControllerBase
{
    private readonly UserManager<Account> _userManager;

    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly AccountRegisterHandler _accountRegisterHandler;

    private readonly AccountAuthorizationHandler _accountAuthorizationHandlers;

    public AuthorizationController(UserManager<Account> userManager, RoleManager<IdentityRole<int>> roleManager,
        AccountRegisterHandler registerHandler, AccountAuthorizationHandler authorizationHandlers)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _accountRegisterHandler = registerHandler ?? throw new ArgumentNullException(nameof(registerHandler));
        _accountAuthorizationHandlers = authorizationHandlers ?? throw new ArgumentNullException(nameof(authorizationHandlers));
    }

    [HttpPost]
    public async Task<IActionResult> Register()
    {
        await _accountRegisterHandler.Register();

        return Ok();
    }
}
