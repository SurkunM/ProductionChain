using Microsoft.AspNetCore.Identity;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Authorization;

public class AccountAuthorizationHandler
{
    private readonly RoleManager<IdentityUserRole<int>> _roleManager;

    private readonly UserManager<Account> _userManager;

    public AccountAuthorizationHandler(RoleManager<IdentityUserRole<int>> roleManager, UserManager<Account> userManager)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }
}
