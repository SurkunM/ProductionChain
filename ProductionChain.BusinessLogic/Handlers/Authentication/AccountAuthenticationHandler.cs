using Microsoft.AspNetCore.Identity;
using ProductionChain.Contracts.Authentication;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Authentication;

public class AccountAuthenticationHandler
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly UserManager<Account> _userManager;

    private readonly SignInManager<Account> _signInManager;

    private readonly IJwtGenerationService _jwtGenerationService;

    public AccountAuthenticationHandler(RoleManager<IdentityRole<int>> roleManager, UserManager<Account> userManager, SignInManager<Account> signInManager,
         IJwtGenerationService jwtGenerationService)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _jwtGenerationService = jwtGenerationService ?? throw new ArgumentNullException(nameof(jwtGenerationService));

    }

    public async Task<AuthLoginResponse> HandleAsync(AuthLoginRequest loginRequest)
    {
        var account = await _userManager.FindByNameAsync(loginRequest.UserLogin);

        if (account is null)
        {
            throw new NotFoundException("Сотрудник под таким логином не зарегистрирован");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(account, loginRequest.Password, false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Не удалось авторизоваться");
        }

        var token = await _jwtGenerationService.GenerateTokenAsync(account);

        return new AuthLoginResponse
        {
            Token = token,
            Code = "test"
        };
    }

    public async Task Logout(string token)
    {
        await _signInManager.SignOutAsync();

        //Сделать: доб. в "ConcurrentDictionary<string, DateTime>"  await _jwtGenerationService.AddToBlacklistAsync(token);
        // проверка (ContainKey) _jwtGenerationService.IsTokenBlacklistedAsync(token)
        //удаление (TryRemove) _jwtGenerationService.RemoveExpiredTokensAsync()
    }
}
