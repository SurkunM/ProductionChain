using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateProductionTaskHandler> _logger;

    public UpdateProductionTaskHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductionTaskHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionTaskRequest taskRequest)
    {
        var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var productionOrder = await productionOrdersRepository.GetByIdAsync(taskRequest.ProductionOrderId);
        var product = await productsRepository.GetByIdAsync(taskRequest.ProductId);
        var employee = await employeesRepository.GetByIdAsync(taskRequest.EmployeeId);

        if (productionOrder is null || product is null || employee is null)
        {
            return false;
        }

        tasksRepository.Update(taskRequest.ToTaskModel(productionOrder, product, employee,
            (ProgressStatusType)taskRequest.StatusId, taskRequest.StartTime));

        return true;
    }
}
