using Microsoft.AspNetCore.Identity;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Authentication;

public class AccountRegisterHandler
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly UserManager<Account> _userManager;

    public AccountRegisterHandler(RoleManager<IdentityRole<int>> roleManager, UserManager<Account> userManager)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task Register()
    {

    }
}
