using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionOrderHandler> _logger;

    public DeleteProductionOrderHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();

            var productionOrder = await productionOrdersRepository.GetByIdAsync(id);

            if (productionOrder is null)
            {
                return false;
            }

            productionOrdersRepository.Delete(productionOrder);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении производственного заказа из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
