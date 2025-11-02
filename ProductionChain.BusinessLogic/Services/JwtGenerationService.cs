using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductionChain.Contracts.IServices;
using ProductionChain.Contracts.Settings;
using ProductionChain.Model.BasicEntities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductionChain.BusinessLogic.Services;

public class JwtGenerationService : IJwtGenerationService
{
    private readonly UserManager<Account> _userManager;

    private readonly JwtSettings _jwtSettings;

    public JwtGenerationService(UserManager<Account> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string> GenerateTokenAsync(Account account)
    {
        var claims = await GenerateClaimsAsync(account);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(_jwtSettings.ExpiryHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<List<Claim>> GenerateClaimsAsync(Account account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Name, account.UserName),
            new("EmployeeId", account.EmployeeId.ToString()),
            new("ChiefId", account.Employee.ChiefId?.ToString()?? "")
        };

        var roles = await _userManager.GetRolesAsync(account);

        claims.AddRange(roles
            .Select(r => new Claim(ClaimTypes.Role, r)));

        return claims;
    }
}
