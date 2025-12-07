using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.Enums;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Delete;

public class DeleteProductionOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionOrderHandler> _logger;

    public DeleteProductionOrderHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(int id)
    {
        var productionOrdersRepository = _unitOfWork.GetRepository<IAssemblyProductionOrdersRepository>();
        var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            var productionOrder = await productionOrdersRepository.GetByIdAsync(id);

            if (productionOrder is null)
            {
                _logger.LogError("Не найден производственный заказ по id={id}", id);

                throw new NotFoundException("Не найден производственный заказ, обновление данных не выполнено");
            }

            if (productionOrdersRepository.HasInProgressTasks(id))
            {
                _logger.LogError("Ошибка! Попытка завершить производственную задачу в которой есть незавершенные задачи.");

                throw new InvalidStateException("Не удалось завершить производственную задачу, найдены незавершенные задачи");
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

            productionOrdersRepository.Delete(productionOrder);

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении производственного заказа из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
