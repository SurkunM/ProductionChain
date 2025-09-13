using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly LoginHandler _loginHandler;

    // private readonly AccountRegisterHandler _accountRegisterHandler;

    public AuthenticationController(UserManager<Account> userManager, RoleManager<IdentityRole<int>> roleManager, LoginHandler loginHandler)
    // AccountRegisterHandler accountRegisterHandler, 
    {
        //_accountRegisterHandler = accountRegisterHandler ?? throw new ArgumentNullException(nameof(accountRegisterHandler));
        _loginHandler = loginHandler ?? throw new ArgumentNullException(nameof(loginHandler));
    }

    [HttpPost]
    public async Task<AuthLoginResponse> Login(AuthLoginRequest loginRequest)
    {
        var result = await _loginHandler.HandleAsync(loginRequest);

        return result;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _loginHandler.Logout();

        return Ok();
    }
}
