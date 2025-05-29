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

    public static ProductionAssemblyTask ToTaskModel(this ProductionTaskRequest taskRequest, ProductionAssemblyOrders productionOrder,
            Product product, Employee employee, ProgressStatusType statusType, DateTime dateTime)
    {
        return new ProductionAssemblyTask
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

    public static ProductionAssemblyHistory ToHistoryModel(this ProductionHistoryRequest historyRequest, ProductionAssemblyTask task)
    {
        return new ProductionAssemblyHistory
        {
            AssemblyTask = task
        };
    }

    public static ProductionAssemblyOrders ToProductionOrderModel(this ProductionOrdersRequest productionOrdersRequest, Order order, Product product)
    {
        return new ProductionAssemblyOrders
        {
            Order = order,
            Product = product,
            Count = productionOrdersRequest.Count,
            StatusType = ProgressStatusType.Pending
        };
    }

    public static ProductionAssemblyWarehouse ToAssemblyWarehouseModel(this AssemblyWarehouseRequest warehouseRequest, Product product)
    {
        return new ProductionAssemblyWarehouse
        {
            Product = product,
            Count = warehouseRequest.Count
        };
    }
}
