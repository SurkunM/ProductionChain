using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionChain.Contracts.Dto;

public class TasksDto
{
    public int Id { get; set; }

    public required string ProductName { get; set; }

    public int Count { get; set; }

    public required string EmployeeName { get; set; }

    public required string Status { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
