using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.BusinessLogic.Handlers.Identity;

public class AccountAuthorizationHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly UserManager<Account> _userManager;

    private readonly ILogger<AccountAuthorizationHandler> _logger;

    public AccountAuthorizationHandler(UserManager<Account> userManager, IUnitOfWork unitOfWork, ILogger<AccountAuthorizationHandler> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(AccountRegisterRequest accountRegisterRequest)
    {
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var employee = await employeeRepository.GetByIdAsync(accountRegisterRequest.EmployeeId) ?? throw new NotFoundException("Сотрудник не найден.");
            var account = accountRegisterRequest.ToAccountModel(employee);

            var result = await _userManager.CreateAsync(account, accountRegisterRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                _logger.LogError("При создании аккаунта произошла ошибка. Errors: {errors}.", errors);

                throw new AccountNotCreatedException("Аккаунт не создан.");
            }

            await _userManager.AddToRoleAsync(account, accountRegisterRequest.Role);

            employeeRepository.AddAccount(employee.Id, account);

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "При регистрации аккаунта произошла ошибка. Транзакция отменена.");

            throw;
        }

    }
}
