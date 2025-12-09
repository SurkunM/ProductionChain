using Microsoft.AspNetCore.Mvc.Filters;
using ProductionChain.Contracts.Exceptions;
using System.Security.Claims;

namespace ProductionChain.BusinessLogic.Attributes;

public class ExtractTokenAndUserIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;

        var header = httpContext.Request.Headers["Authorization"].ToString() ?? throw new NotFoundException("Токен не найден в заголовке авторизации");
        var token = header["Bearer ".Length..].Trim();

        var accountIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NotFoundException("Не найден id аккаунта в заголовке");

        var accountId = int.Parse(accountIdClaim);

        context.ActionArguments["token"] = token;
        context.ActionArguments["accountId"] = accountId;

        base.OnActionExecuting(context);
    }
}
