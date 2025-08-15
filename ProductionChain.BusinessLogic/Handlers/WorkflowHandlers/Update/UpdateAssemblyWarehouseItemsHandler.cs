using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateAssemblyWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateAssemblyWarehouseItemsHandler> _logger;

    public UpdateAssemblyWarehouseItemsHandler(IUnitOfWork unitOfWork, ILogger<UpdateAssemblyWarehouseItemsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(AssemblyWarehouseItemResponse projectDto)
    {
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        employeesRepository.Update(requestDto.ToModel());

        await _unitOfWork.SaveAsync();
    }
}
