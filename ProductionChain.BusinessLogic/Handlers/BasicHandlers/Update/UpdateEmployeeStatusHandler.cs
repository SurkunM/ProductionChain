using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;

public class UpdateEmployeeStatusHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateEmployeeStatusHandler> _logger;

    public UpdateEmployeeStatusHandler(IUnitOfWork unitOfWork, ILogger<UpdateEmployeeStatusHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(EmployeeStateRequest employeeRequest)
    {
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var employee = await employeesRepository.GetByIdAsync(employeeRequest.Id);

        if (employee is null || !Enum.IsDefined(typeof(EmployeeStatusType), employeeRequest.StatusType))
        {
            _logger.LogError("При попытке изменения статуса сотрудника произошла ошибка. Передан несуществующий id или statusType");

            return false;
        }

        employee.Status = (EmployeeStatusType)employeeRequest.StatusType;

        return true;
    }
}
