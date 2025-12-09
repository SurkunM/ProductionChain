using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Attributes;
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
        var result = await _loginHandler.LoginAsync(loginRequest);

        return result;
    }

    [HttpPost]
    [Authorize]
    [ExtractTokenAndUserId]
    public IActionResult Logout(string token, int accountId)
    {
        _loginHandler.Logout(token, accountId);

        return Ok();
    }
}
