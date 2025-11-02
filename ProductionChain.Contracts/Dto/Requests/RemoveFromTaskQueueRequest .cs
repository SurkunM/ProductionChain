namespace ProductionChain.Contracts.Dto.Requests;

public class RemoveFromTaskQueueRequest
{
    public int EmployeeId { get; set; }

    public int TaskId { get; set; }
}
