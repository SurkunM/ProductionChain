using Microsoft.AspNetCore.Identity;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Authorization;

public class AccountAuthorizationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly RoleManager<IdentityRole<int>> _roleManager;

    private readonly UserManager<Account> _userManager;

    public AccountAuthorizationHandler(RoleManager<IdentityRole<int>> roleManager, UserManager<Account> userManager,
        IUnitOfWork unitOfWork)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task HandleAsync(AuthAccountRegisterRequest accountRegisterRequest)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var employee = await employeeRepository.GetByIdAsync(accountRegisterRequest.EmployeeId);

        if (employee is null)
        {
            throw new NotFoundException("Сотрудник не найден");
        }

        var adminAccount = new Account//Сделать маппинг
        {
            UserName = accountRegisterRequest.Login,
            Employee = employee
        };

        var result = await _userManager.CreateAsync(adminAccount, accountRegisterRequest.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(adminAccount, accountRegisterRequest.Role);
        }

        //var errors = string.Join(", ", result.Errors.Select(e => e.Description));

        //throw new Exception($"Ошибка создания пользователя: {errors}");//Сделать кастомный exception
    }
}
