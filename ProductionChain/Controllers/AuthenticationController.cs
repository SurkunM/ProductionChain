using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Authentication;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly AccountAuthenticationHandler _loginHandler;

    public AuthenticationController(UserManager<Account> userManager, RoleManager<IdentityRole<int>> roleManager, AccountAuthenticationHandler loginHandler)
    {
        _loginHandler = loginHandler ?? throw new ArgumentNullException(nameof(loginHandler));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<AuthenticationLoginResponse> Login(AuthenticationLoginRequest loginRequest)
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
