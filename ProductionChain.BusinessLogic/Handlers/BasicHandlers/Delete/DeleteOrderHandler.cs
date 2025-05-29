using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;

public class DeleteOrderHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteOrderHandler> _logger;

    public DeleteOrderHandler(IUnitOfWork unitOfWork, ILogger<DeleteOrderHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var ordersRepository = _unitOfWork.GetRepository<IOrdersRepository>();

            var order = await ordersRepository.GetByIdAsync(id);

            if (order is null)
            {
                return false;
            }

            ordersRepository.Delete(order);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении заказа из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
