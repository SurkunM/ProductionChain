using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.Mapping;

public static class MappingExtensions
{
    public static Order ToOrderModel(this OrderRequest orderRequest, Product product)
    {
        return new Order
        {
            Id = orderRequest.Id,
            Customer = orderRequest.Customer,
            Product = product,
            Count = orderRequest.Count,
            StageType = ProgressStatusType.InProgress.ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static AssemblyProductionTask ToTaskModel(this ProductionTaskRequest taskRequest, AssemblyProductionOrders productionOrder,
            Product product, Employee employee, ProgressStatusType statusType, DateTime dateTime)
    {
        return new AssemblyProductionTask
        {
            Id = taskRequest.Id,
            ProductionOrder = productionOrder,
            Employee = employee,
            Product = product,
            Count = taskRequest.Count,
            ProgressStatus = statusType,
            StartTime = dateTime
        };
    }

    public static ProductionHistory ToHistoryModel(this ProductionHistoryRequest historyRequest, AssemblyProductionTask task)
    {
        return new ProductionHistory
        {
            AssemblyTask = task
        };
    }

    public static AssemblyProductionOrders ToProductionOrderModel(this ProductionOrdersRequest productionOrdersRequest, Order order, Product product, ProgressStatusType status)
    {
        return new AssemblyProductionOrders
        {
            Order = order,
            Product = product,
            TotalCount = productionOrdersRequest.Count,
            StatusType = status
        };
    }

    public static AssemblyProductionWarehouse ToAssemblyWarehouseModel(this AssemblyWarehouseRequest warehouseRequest, Product product)
    {
        return new AssemblyProductionWarehouse
        {
            Product = product,
            Count = warehouseRequest.Count
        };
    }
}
