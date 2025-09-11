using Microsoft.AspNetCore.Identity;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Authentication;

public class LoginHandler
{
    private readonly RoleManager<IdentityUserRole<int>> _roleManager;

    private readonly UserManager<Account> _userManager;

    private readonly SignInManager<Account> _signInManager;

    public LoginHandler(RoleManager<IdentityUserRole<int>> roleManager, UserManager<Account> userManager, SignInManager<Account> signInManager)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task Login()
    {

    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }
}
