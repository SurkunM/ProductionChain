using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using ProductionChain.BusinessLogic.Services;
using ProductionChain.Contracts.Settings;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductionChain.Tests.Services;

public class JwtGenerationServiceTests
{
    private readonly Mock<UserManager<Account>> _userManagerMock;

    private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;

    private readonly JwtSettings _jwtSettings;

    private readonly JwtGenerationService _jwtService;

    private readonly Employee _employee;

    private readonly Account _account;

    public JwtGenerationServiceTests()
    {
        _userManagerMock = new Mock<UserManager<Account>>(Mock.Of<IUserStore<Account>>(),
            null, null, null, null, null, null, null, null);

        _jwtSettings = new JwtSettings
        {
            SecretKey = "SuperSecretKeyForTestingPurposesOnly123!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryHours = 8
        };

        _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
        _jwtOptionsMock.Setup(x => x.Value).Returns(_jwtSettings);

        _jwtService = new JwtGenerationService(_userManagerMock.Object, _jwtOptionsMock.Object);

        _employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        _account = new Account
        {
            Id = 2,
            UserName = "UserTestLogin",
            Employee = _employee,
            EmployeeId = _employee.Id
        };
    }

    [Fact]
    public async Task Should_Successfully_GenerateTokenAsync_ReturnsValidToken()
    {
        var roles = new List<string> { "Manager" };

        _userManagerMock.Setup(x => x.GetRolesAsync(_account)).ReturnsAsync(roles);

        var token = await _jwtService.GenerateTokenAsync(_account);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("2", GetClaimValue(jwtToken, ClaimTypes.NameIdentifier));
        Assert.Equal("UserTestLogin", GetClaimValue(jwtToken, ClaimTypes.Name));
        Assert.Equal("1", GetClaimValue(jwtToken, "EmployeeId"));
        Assert.Equal(string.Empty, GetClaimValue(jwtToken, "ChiefId"));

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        Assert.Single(roleClaims);
        Assert.Contains(roleClaims, c => c.Value == "Manager");
    }

    [Fact]
    public async Task Should_RolesIsEmpty_When_NoRoles()
    {
        _userManagerMock.Setup(x => x.GetRolesAsync(_account)).ReturnsAsync(new List<string>());

        var token = await _jwtService.GenerateTokenAsync(_account);

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        Assert.Empty(roleClaims);
    }

    [Fact]
    public async Task Should_ArgumentNullException_When_RolesIsNull()
    {
        _userManagerMock.Setup(x => x.GetRolesAsync(_account)).ReturnsAsync((IList<string>)null!);

        await Assert.ThrowsAsync<ArgumentNullException>(() => _jwtService.GenerateTokenAsync(_account));
    }

    [Fact]
    public async Task Should_Successfully_GenerateTokenAsync_ReturnsProperlyStructuredJwtToken()
    {
        _userManagerMock.Setup(x => x.GetRolesAsync(_account)).ReturnsAsync(new List<string> { "User" });

        var token = await _jwtService.GenerateTokenAsync(_account);

        var handler = new JwtSecurityTokenHandler();

        Assert.True(handler.CanReadToken(token));

        var jwtToken = handler.ReadJwtToken(token);

        Assert.NotNull(jwtToken.Header);
        Assert.NotNull(jwtToken.Payload);
        Assert.Equal("HS256", jwtToken.Header.Alg);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var principal = handler.ValidateToken(token, validationParameters, out _);

        Assert.NotNull(principal);
    }

    private static string? GetClaimValue(JwtSecurityToken token, string claimType)
    {
        return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
