using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductionTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> HandleAsync(ProductionTaskRequest taskRequest)
    {
        _unitOfWork.BeginTransaction();

        var tasksRepository = _unitOfWork.GetRepository<IProductionAssemblyTasksRepository>();
        var productionOrdersRepository = _unitOfWork.GetRepository<IProductionAssemblyOrdersRepository>();
        var productsRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var employeesRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        var productionOrder = await productionOrdersRepository.GetByIdAsync(taskRequest.ProductionOrderId);
        var product = await productsRepository.GetByIdAsync(taskRequest.ProductId);
        var employee = await employeesRepository.GetByIdAsync(taskRequest.EmployeeId);

        if (productionOrder is null || product is null || employee is null)
        {
            return false;
        }

        await tasksRepository.CreateAsync(taskRequest.ToTaskModel(productionOrder, product, employee,
            Model.Enums.ProgressStatusType.Pending, DateTime.UtcNow));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
