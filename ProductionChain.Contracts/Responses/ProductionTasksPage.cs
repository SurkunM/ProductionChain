using ProductionChain.Contracts.Dto;

namespace ProductionChain.Contracts.Responses;

public class ProductionTasksPage
{
    public List<TasksDto> Tasks { get; set; } = new List<TasksDto>();

    public int Total { get; set; }
}
