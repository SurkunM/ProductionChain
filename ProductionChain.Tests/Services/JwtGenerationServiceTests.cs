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
    }

    [Fact]
    public async Task GenerateTokenAsync_WithValidAccount_ReturnsValidTokenWithCorrectClaims()
    {
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        var account = new Account
        {
            Id = 123,
            UserName = "Tester",
            Employee = employee,
            EmployeeId = employee.Id
        };

        var roles = new List<string> { "Admin", "Manager" };

        _userManagerMock.Setup(x => x.GetRolesAsync(account))
            .ReturnsAsync(roles);

        // Act
        var token = await _jwtService.GenerateTokenAsync(account);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Декодируем токен для проверки claims
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Проверяем стандартные claims
        Assert.Equal(account.Id.ToString(), GetClaimValue(jwtToken, ClaimTypes.NameIdentifier));
        Assert.Equal(account.UserName, GetClaimValue(jwtToken, ClaimTypes.Name));
        Assert.Equal(account.EmployeeId.ToString(), GetClaimValue(jwtToken, "EmployeeId"));

        // Проверяем роли
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains(roleClaims, c => c.Value == "Admin");
        Assert.Contains(roleClaims, c => c.Value == "Manager");

        // Проверяем issuer и audience
        Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
        Assert.Equal(_jwtSettings.Audience, jwtToken.Audiences.First());

        // Проверяем expiry
        var expectedExpiry = DateTime.Now.AddHours(_jwtSettings.ExpiryHours);
        Assert.True(jwtToken.ValidTo <= expectedExpiry.AddMinutes(1));
        Assert.True(jwtToken.ValidTo >= expectedExpiry.AddMinutes(-1));
    }

    [Fact]
    public async Task GenerateTokenAsync_WithNoRoles_ReturnsTokenWithoutRoleClaims()
    {
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        // Arrange
        var account = new Account { Id = 1, UserName = "user", Employee = employee, EmployeeId = 1 };

        _userManagerMock.Setup(x => x.GetRolesAsync(account))
            .ReturnsAsync(new List<string>());

        // Act
        var token = await _jwtService.GenerateTokenAsync(account);

        // Assert
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        Assert.Empty(roleClaims);
    }

    [Fact]
    public async Task GenerateTokenAsync_WithNullRoles_ReturnsTokenWithoutRoleClaims()
    {
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        // Arrange
        var account = new Account { Id = 1, UserName = "user", Employee = employee, EmployeeId = 1 };

        _userManagerMock.Setup(x => x.GetRolesAsync(account))
            .ReturnsAsync((IList<string>)null);

        // Act
        var token = await _jwtService.GenerateTokenAsync(account);

        // Assert
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        Assert.Empty(roleClaims);
    }

    [Fact]
    public async Task GenerateTokenAsync_ReturnsProperlyStructuredJwtToken()
    {
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Employee1_FirstName",
            LastName = "Employee1_LastName",
            MiddleName = "Employee1_MiddleName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Busy
        };

        // Arrange
        var account = new Account { Id = 1, UserName = "test", Employee = employee, EmployeeId = 1 };
        _userManagerMock.Setup(x => x.GetRolesAsync(account))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var token = await _jwtService.GenerateTokenAsync(account);

        // Assert
        var handler = new JwtSecurityTokenHandler();

        // Проверяем, что токен может быть прочитан
        Assert.True(handler.CanReadToken(token));

        var jwtToken = handler.ReadJwtToken(token);

        // Проверяем наличие обязательных частей JWT
        Assert.NotNull(jwtToken.Header);
        Assert.NotNull(jwtToken.Payload);
        Assert.Equal("HS256", jwtToken.Header.Alg);

        // Проверяем подпись
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        // Должен пройти валидацию подписи
        var principal = handler.ValidateToken(token, validationParameters, out _);
        Assert.NotNull(principal);
    }

    private string GetClaimValue(JwtSecurityToken token, string claimType)
    {
        return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
