﻿using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Model.WorkflowEntities;

public class AssemblyProductionTask
{
    public int Id { get; set; }

    public int ProductionOrderId { get; set; }

    public virtual required AssemblyProductionOrder ProductionOrder { get; set; }

    public int ProductId { get; set; }

    public virtual required Product Product { get; set; }

    public int ProductsCount { get; set; }

    public int EmployeeId { get; set; }

    public virtual required Employee Employee { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
