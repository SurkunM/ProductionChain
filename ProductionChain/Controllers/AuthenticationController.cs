using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthenticationController : ControllerBase
{
    private readonly AccountAuthenticationHandler _loginHandler;

    public AuthenticationController(AccountAuthenticationHandler loginHandler)
    {
        _loginHandler = loginHandler ?? throw new ArgumentNullException(nameof(loginHandler));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<AccountLoginResponse> Login(AccountLoginRequest loginRequest)
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
