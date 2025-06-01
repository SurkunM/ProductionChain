using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionHistoryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateProductionHistoryHandler> _logger;

    public CreateProductionHistoryHandler(IUnitOfWork unitOfWork, ILogger<CreateProductionHistoryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ProductionHistoryRequest historyDto)
    {
        _unitOfWork.BeginTransaction();

        var historiesRepository = _unitOfWork.GetRepository<IProductionHistoryRepository>();
        var tasksRepositories = _unitOfWork.GetRepository<IAssemblyProductionTasksRepository>();

        var task = await tasksRepositories.GetByIdAsync(historyDto.Id);

        if (task is null)
        {
            _logger.LogError("");

            return false;
        }

        await historiesRepository.CreateAsync(historyDto.ToHistoryModel(task));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
