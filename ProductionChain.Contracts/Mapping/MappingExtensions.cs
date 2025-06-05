using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.Mapping;

public static class MappingExtensions
{
    public static AssemblyProductionTask ToTaskModel(this ProductionTaskRequest taskRequest, AssemblyProductionOrders productionOrder,
            Product product, Employee employee, ProgressStatusType statusType, DateTime dateTime)
    {
        return new AssemblyProductionTask
        {
            Id = taskRequest.Id,
            ProductionOrder = productionOrder,
            Employee = employee,
            Product = product,
            ProductsCount = taskRequest.ProductsCount,
            ProgressStatus = statusType,
            StartTime = dateTime
        };
    }

    public static ProductionHistory ToHistoryModel(this AssemblyProductionTask task)
    {
        return new ProductionHistory
        {
            ProductionTaskId = task.Id,
            Employee = $"{task.Employee.LastName} {task.Employee.FirstName} {task.Employee.MiddleName ?? ""}".Trim(),
            Product = $"{task.Product.Name} ({task.Product.Model})",
            ProductsCount = task.ProductsCount,
            StartTime = task.StartTime,
            EndTime = DateTime.UtcNow
        };
    }

    public static AssemblyProductionOrders ToProductionOrderModel(this ProductionOrdersRequest productionOrdersRequest, Order order, Product product)
    {
        var requiredProductCount = productionOrdersRequest.ProductsCount - productionOrdersRequest.AvailableCount;
        var status = ProgressStatusType.InProgress;

        if (requiredProductCount <= 0)
        {
            requiredProductCount = 0;
            status = ProgressStatusType.Done;
        }

        return new AssemblyProductionOrders
        {
            Order = order,
            Product = product,
            TotalProductsCount = requiredProductCount,
            StatusType = status
        };
    }
}
