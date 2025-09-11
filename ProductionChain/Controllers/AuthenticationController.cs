using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<Account> _userManager;

    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly LoginHandler _loginHandler;

   // private readonly AccountRegisterHandler _accountRegisterHandler;

    public AuthenticationController(UserManager<Account> userManager, RoleManager<IdentityRole<int>> roleManager,LoginHandler loginHandler)
       // AccountRegisterHandler accountRegisterHandler, 
    {
        //_accountRegisterHandler = accountRegisterHandler ?? throw new ArgumentNullException(nameof(accountRegisterHandler));
        _loginHandler = loginHandler ?? throw new ArgumentNullException(nameof(loginHandler));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    [HttpPost]
    public async Task<IActionResult> Login()
    {
        await _loginHandler.Login();

        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _loginHandler.Logout();

        return Ok();
    }
}
