using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Contracts.IServices;

namespace ProductionChain.BusinessLogic.Handlers.Workflow.Get;

public class GetTaskQueueHandler
{
    private readonly ITaskQueueService _tasksQueueService;

    public GetTaskQueueHandler(ITaskQueueService tasksQueueService)
    {
        _tasksQueueService = tasksQueueService ?? throw new ArgumentNullException(nameof(tasksQueueService));
    }

    public List<TaskQueueDto> GetTaskQueueHandle()
    {
        return _tasksQueueService.GetTaskQueueToList();
    }
}
