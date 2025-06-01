using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;

public class DeleteProductionTaskHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteProductionTaskHandler> _logger;

    public DeleteProductionTaskHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductionTaskHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(int id)
    {
        try
        {
            _unitOfWork.BeginTransaction();
            var tasksRepository = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();

            var task = await tasksRepository.GetByIdAsync(id);

            if (task is null)
            {
                return false;
            }

            tasksRepository.Delete(task);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении задачи из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }
}
