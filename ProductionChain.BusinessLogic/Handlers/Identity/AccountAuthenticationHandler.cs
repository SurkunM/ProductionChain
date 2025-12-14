using Microsoft.AspNetCore.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Identity;

public class AccountAuthenticationHandler
{
    private readonly UserManager<Account> _userManager;

    private readonly IJwtGenerationService _jwtGenerationService;

    private readonly ITokenBlacklistService _tokenBlacklistService;

    public AccountAuthenticationHandler(UserManager<Account> userManager, SignInManager<Account> signInManager,
        IJwtGenerationService jwtGenerationService, ITokenBlacklistService tokenBlacklistService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _jwtGenerationService = jwtGenerationService ?? throw new ArgumentNullException(nameof(jwtGenerationService));
        _tokenBlacklistService = tokenBlacklistService ?? throw new ArgumentNullException(nameof(tokenBlacklistService));
    }

    public async Task<AccountLoginResponse> LoginAsync(AccountLoginRequest loginRequest)
    {
        var account = await _userManager.FindByNameAsync(loginRequest.UserLogin) ?? throw new NotFoundException("Сотрудник под таким логином не зарегистрирован");
        var result = await _userManager.CheckPasswordAsync(account, loginRequest.Password);

        if (!result)
        {
            throw new InvalidCredentialsException("Не верный логин или пароль");
        }

        var token = await _jwtGenerationService.GenerateTokenAsync(account);

        var roles = await _userManager.GetRolesAsync(account);

        return new AccountLoginResponse
        {
            Token = token,
            UserData = account.ToEmployeeDataResponse(roles)
        };
    }

    public void Logout(string token, int accountId)
    {
        _tokenBlacklistService.SetBlacklistTokenAsync(token, accountId);
    }
}
