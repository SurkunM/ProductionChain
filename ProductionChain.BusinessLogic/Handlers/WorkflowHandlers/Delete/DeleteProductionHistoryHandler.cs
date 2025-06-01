using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteProductionHistoryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionHistoryHandler> _logger;

    public DeleteProductionHistoryHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionHistoryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var historiesRepository = _unitOfWork.GetRepository<IProductionHistoryRepository>();

            var history = await historiesRepository.GetByIdAsync(id);

            if (history is null)
            {
                return false;
            }

            historiesRepository.Delete(history);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении истории из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
