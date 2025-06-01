using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteAssemblyWarehouseItemHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteAssemblyWarehouseItemHandler> _logger;

    public DeleteAssemblyWarehouseItemHandler(IUnitOfWork unitOfWork, ILogger<DeleteAssemblyWarehouseItemHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var assemblyWarehouseRepository = _unitOfWork.GetRepository<IAssemblyProductionWarehouseRepository>();

            var assemblyWarehouseItem = await assemblyWarehouseRepository.GetByIdAsync(id);

            if (assemblyWarehouseItem is null)
            {
                return false;
            }

            assemblyWarehouseRepository.Delete(assemblyWarehouseItem);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении позиции в складе сборки произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
