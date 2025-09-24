using Microsoft.AspNetCore.Http;
using ProductionChain.Contracts.Dto.Contexts;
using ProductionChain.Contracts.Exceptions;
using System.Security.Claims;

namespace ProductionChain.Contracts.Mapping;

public static class ClaimsMappingExtensions
{
    public static string GetBearerToken(this HttpContext httpContext)
    {
        var header = httpContext.Request.Headers["Authorization"].ToString();

        return header.Replace("Bearer", "").Trim();
    }

    public static AccountContext ToAccountContext(this ClaimsPrincipal principal)
    {
        return new AccountContext
        {
            AccountId = principal.GetAccountIdAsInt(),
            Role = principal.FindFirst(ClaimTypes.Role)?.Value ?? throw new NotFoundException("Не задана роль аккаунта."),
            EmployeeId = principal.GetEmployeeIdAsInt()
        };
    }

    public static int GetAccountIdAsInt(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(claimValue, out int accountId))
        {
            return accountId;
        }

        throw new NotFoundException("Аккаунт с таким id не найден.");
    }

    public static int GetEmployeeIdAsInt(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirst("EmployeeId")?.Value;

        if (int.TryParse(claimValue, out int employeeId))
        {
            return employeeId;
        }

        throw new NotFoundException("Сотрудник с таким id не найден.");
    }
}
