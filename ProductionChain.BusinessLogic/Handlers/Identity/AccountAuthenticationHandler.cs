using Microsoft.AspNetCore.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Identity;

public class AccountAuthenticationHandler
{
    private readonly UserManager<Account> _userManager;

    private readonly SignInManager<Account> _signInManager;

    private readonly IJwtGenerationService _jwtGenerationService;

    public AccountAuthenticationHandler(UserManager<Account> userManager, SignInManager<Account> signInManager, IJwtGenerationService jwtGenerationService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _jwtGenerationService = jwtGenerationService ?? throw new ArgumentNullException(nameof(jwtGenerationService));
    }

    public async Task<AuthenticationLoginResponse> HandleAsync(AuthenticationLoginRequest loginRequest)
    {
        var account = await _userManager.FindByNameAsync(loginRequest.UserLogin);

        if (account is null)
        {
            throw new NotFoundException("Сотрудник под таким логином не зарегистрирован");
        }

        var result = await _userManager.CheckPasswordAsync(account, loginRequest.Password);

        if (!result)
        {
            throw new UnauthorizedAccessException("Не удалось авторизоваться");
        }

        var token = await _jwtGenerationService.GenerateTokenAsync(account);
        var roles = await _userManager.GetRolesAsync(account);

        var employeeData = new EmployeeData//может перенести в метод в employeeRepository
        {
            UserId = account.EmployeeId,
            UserName = $"{account.Employee.LastName} {account.Employee.FirstName[0]}. " +
            $"{(account.Employee.MiddleName is null ? " " : account.Employee.MiddleName[0].ToString() + ".")}",
            Roles = roles.ToList()
        };

        return new AuthenticationLoginResponse
        {
            Token = token,
            UserData = employeeData
        };
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }
}
