using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

public class UpdateAssemblyWarehouseItemHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateAssemblyWarehouseItemHandler> _logger;

    public UpdateAssemblyWarehouseItemHandler(IUnitOfWork unitOfWork, ILogger<UpdateAssemblyWarehouseItemHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(AssemblyWarehouseRequest assemblyWarehouseRequest)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var assemblyWarehouseRepository = _unitOfWork.GetRepository<IProductionAssemblyWarehouseRepository>();

            var assemblyWarehouseItem = await assemblyWarehouseRepository.GetByIdAsync(assemblyWarehouseRequest.Id);

            if (assemblyWarehouseItem is null)
            {
                return false;
            }

            if (assemblyWarehouseRequest.AddCount > 0)
            {
                assemblyWarehouseItem.Count += assemblyWarehouseRequest.AddCount;
            }
            else if (assemblyWarehouseRequest.SubtractCount > 0)
            {
                assemblyWarehouseItem.Count -= assemblyWarehouseRequest.SubtractCount;
            }
            else
            {
                _logger.LogError("Переданы пустые значения AddCount/SubtractCount. Изменения в складе сборки не выполнены.");

                return false;
            }

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при изменения количества продукции в складе сборки");

            _unitOfWork.RollbackTransaction();

            return false;
        }
    }
}
