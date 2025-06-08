using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.Enums;

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
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();
        var assemblyWarehouse = _unitOfWork.GetRepository<IAssemblyProductionWarehouseRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var productionOrder = await productionOrdersRepository.GetByIdAsync(id);

            if (productionOrder is null)
            {
                _logger.LogError("Не найден производственный заказ по id={id}", id);

                return false;
            }

            if (productionOrdersRepository.HasInProgressTasks(id))
            {
                _logger.LogError("Ошибка! Попытка завершить производственную задачу в которой есть незавершенные задачи.");

                return false;
            }

            ordersRepository.SetAvailableProductsCount(productionOrder.OrderId, productionOrder.CompletedProductsCount);

            if (productionOrdersRepository.IsCompleted(id))
            {
                ordersRepository.UpdateOrderStatus(productionOrder.OrderId, ProgressStatusType.Done);
            }
            else
            {
                ordersRepository.UpdateOrderStatus(productionOrder.OrderId, ProgressStatusType.Pending);
            }

            var success = assemblyWarehouse.AddWarehouseItems(productionOrder.ProductId, productionOrder.CompletedProductsCount);

            if (!success)
            {
                _logger.LogError("При добавлении собранной продукции в склад ГП произошла ошибка.");

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
