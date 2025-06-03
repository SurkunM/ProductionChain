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
            ProductsCount = orderRequest.ProductsCount,
            StageType = ProgressStatusType.InProgress,
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
            ProductsCount = taskRequest.ProductsCount,
            ProgressStatus = statusType,
            StartTime = dateTime
        };
    }

    public static ProductionHistory ToHistoryModel(this AssemblyProductionTask task)
    {
        return new ProductionHistory
        {
            ProductionTask = task,
            Employee = $"{task.Employee.LastName} {task.Employee.FirstName} {task.Employee.MiddleName ?? ""}".Trim(),
            Product = $"{task.Product.Name} ({task.Product.Model})",
            ProductsCount = task.ProductsCount
        };
    }

    public static AssemblyProductionOrders ToProductionOrderModel(this ProductionOrdersRequest productionOrdersRequest, Order order, Product product, ProgressStatusType status)
    {
        return new AssemblyProductionOrders
        {
            Order = order,
            Product = product,
            TotalProductsCount = productionOrdersRequest.ProductsCount,
            StatusType = status
        };
    }

    public static AssemblyProductionWarehouse ToAssemblyWarehouseModel(this AssemblyWarehouseRequest warehouseRequest, Product product)
    {
        return new AssemblyProductionWarehouse
        {
            Product = product,
            ProductsCount = warehouseRequest.ProductsCount
        };
    }
}
