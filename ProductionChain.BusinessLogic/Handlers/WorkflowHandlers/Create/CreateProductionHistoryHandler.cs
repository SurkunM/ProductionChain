using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.Mapping;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;

public class CreateProductionHistoryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductionHistoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> HandleAsync(ProductionHistoryRequest historyDto)
    {
        _unitOfWork.BeginTransaction();

        var historiesRepository = _unitOfWork.GetRepository<IProductionAssemblyHistoryRepository>();
        var tasksRepositories = _unitOfWork.GetRepository<IProductionAssemblyTasksRepository>();

        var task = await tasksRepositories.GetByIdAsync(historyDto.Id);

        if (task is null)
        {
            return false;
        }

        await historiesRepository.CreateAsync(historyDto.ToHistoryModel(task));

        await _unitOfWork.SaveAsync();

        return true;
    }
}
