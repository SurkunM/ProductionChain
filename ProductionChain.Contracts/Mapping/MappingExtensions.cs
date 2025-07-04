﻿using ProductionChain.Contracts.Dto.Requests;
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
}
