using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateComponentsWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateComponentsWarehouseItemsHandler> _logger;

    public UpdateComponentsWarehouseItemsHandler(IUnitOfWork unitOfWork, ILogger<UpdateComponentsWarehouseItemsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ComponentsWarehouseResponse componentsWarehouseDto)
    {
        var projectsRepository = _unitOfWork.GetRepository<IProductsRepository>();
        var employeeRepository = _unitOfWork.GetRepository<IEmployeesRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var manager = await employeeRepository.GetEmployeesAsync(componentsWarehouseDto.Id);

            if (manager is null)
            {
                _unitOfWork.RollbackTransaction();

                return false;
            }

            projectsRepository.Update(projectDto.ToModel(manager));

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create project. Transaction rolled back");

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}
