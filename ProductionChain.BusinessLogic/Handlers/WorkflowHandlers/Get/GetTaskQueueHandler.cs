using ProductionChain.Contracts.IServices;
using ProductionChain.Model.BasicEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;

public class GetTaskQueueHandler
{
    private readonly ITaskQueueService _tasksQueueService;

    public GetTaskQueueHandler(ITaskQueueService tasksQueueService)
    {
        _tasksQueueService = tasksQueueService ?? throw new ArgumentNullException(nameof(tasksQueueService));
    }

    public List<Employee> GetTaskQueueHandle()
    {
        return _tasksQueueService.GetTaskQueueToList();
    }

    public Employee GetNextEmployeeHandle()
    {
        return _tasksQueueService.GetNextEmployee();
    }
}
