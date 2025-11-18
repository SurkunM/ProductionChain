using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Dto.Shared;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Contracts.Mapping;

public static class MappingExtensions
{
    public static AssemblyProductionTask ToTaskModel(this ProductionTaskRequest taskRequest, AssemblyProductionOrder productionOrder,
            Product product, Employee employee, DateTime dateTime)
    {
        return new AssemblyProductionTask
        {
            Id = taskRequest.Id,
            ProductionOrder = productionOrder,
            Employee = employee,
            Product = product,
            ProductsCount = taskRequest.ProductsCount,
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

    public static AssemblyProductionOrder ToProductionOrderModel(this Order order, Product product)
    {
        var requiredProductCount = order.OrderedProductsCount - order.AvailableProductsCount;
        var status = ProgressStatusType.InProgress;

        if (requiredProductCount <= 0)
        {
            requiredProductCount = 0;
            status = ProgressStatusType.Done;
        }

        return new AssemblyProductionOrder
        {
            Order = order,
            Product = product,
            TotalProductsCount = requiredProductCount,
            StatusType = status
        };
    }

    public static AssemblyProductionWarehouse ToAssemblyWarehouseModel(this AssemblyWarehouseRequest request, Product product)
    {
        return new AssemblyProductionWarehouse
        {
            Id = request.Id,
            Product = product,
            ProductId = product.Id,
            ProductsCount = request.ProductsCount
        };
    }

    public static ComponentsWarehouse ToComponentsWarehouseModel(this ComponentsWarehouseRequest request, Product product)
    {
        return new ComponentsWarehouse
        {
            Id = request.Id,
            Product = product,
            ProductId = request.ProductId,
            Type = request.Type,
            ComponentsCount = request.ComponentsCount
        };
    }

    public static Account ToAccountModel(this AccountRegisterRequest accountRegisterRequest, Employee employee)
    {
        return new Account
        {
            UserName = accountRegisterRequest.Login,
            Employee = employee
        };
    }

    public static TaskQueueDto ToTaskQueueDto(this Employee employee)
    {
        return new TaskQueueDto
        {
            EmployeeId = employee.Id,
            EmployeeFullName = $"{employee.LastName} {employee.FirstName[0]}.{employee.MiddleName?[0]}",
            Position = employee.Position.ToString(),
            CreateDate = DateTime.UtcNow
        };
    }

    public static TaskQueueDto ToTaskQueueDto(this AssemblyProductionTask task)
    {
        return new TaskQueueDto
        {
            TaskProductName = task.Product.Name,
            ProductCount = task.ProductsCount,
            CreateDate = DateTime.UtcNow
        };
    }
}
