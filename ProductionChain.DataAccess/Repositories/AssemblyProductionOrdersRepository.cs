using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class AssemblyProductionOrdersRepository : BaseEfRepository<AssemblyProductionOrders>, IAssemblyProductionOrdersRepository
{
    private readonly ILogger<AssemblyProductionOrdersRepository> _logger;

    private const string _defaultPropertyBySorting = "Product.Name";

    public AssemblyProductionOrdersRepository(ProductionChainDbContext dbContext, ILogger<AssemblyProductionOrdersRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<ProductionOrdersPage> GetProductionOrdersAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(po => po.Product.Name.Contains(queryParameters.Term)
                || po.Product.Model.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var productionOrdersDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(po => new ProductionOrderResponse
            {
                Id = po.Id,
                ProductId = po.ProductId,
                ProductName = po.Product.Name,
                ProductModel = po.Product.Model,
                InProgressCount = po.InProgressProductsCount,
                CompletedCount = po.CompletedProductsCount,
                TotalCount = po.TotalProductsCount,
                Status = po.StatusType.ToString()
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = productionOrdersDtoSorted.Count;
        }

        return new ProductionOrdersPage
        {
            ProductionOrders = productionOrdersDtoSorted,
            Total = totalCount
        };
    }

    public bool AddInProgressCount(int productionOrderId, int inProgressCount)
    {
        var productionOrder = DbSet.FirstOrDefault(po => po.Id == productionOrderId);

        if (productionOrder is null)
        {
            return false;
        }

        productionOrder.InProgressProductsCount += inProgressCount;

        return true;
    }

    public bool SubtractInProgressCount(int orderId, int inProgressCount)
    {
        var productionOrder = DbSet.FirstOrDefault(po => po.Id == orderId);

        if (productionOrder is null)
        {
            return false;
        }

        productionOrder.InProgressProductsCount -= inProgressCount;

        return true;
    }

    public bool AddCompletedCount(int productionOrderId, int completedCount)
    {
        var productionOrder = DbSet.FirstOrDefault(po => po.Id == productionOrderId);

        if (productionOrder is null)
        {
            return false;
        }

        productionOrder.CompletedProductsCount += completedCount;

        return true;
    }

    public bool UpdateProductionOrderStatusById(int productionOrderId)
    {
        var productionOrder = DbSet.FirstOrDefault(po => po.Id == productionOrderId);

        if (productionOrder is null)
        {
            return false;
        }

        if (productionOrder.InProgressProductsCount > 0)
        {
            productionOrder.StatusType = ProgressStatusType.InProgress;
        }
        else if (productionOrder.CompletedProductsCount == productionOrder.TotalProductsCount)
        {
            productionOrder.StatusType = ProgressStatusType.Done;
        }
        else
        {
            productionOrder.StatusType = ProgressStatusType.Pending;
        }

        return true;
    }

    private Expression<Func<AssemblyProductionOrders, object>> CreateSortExpression(string propertyName)
    {
        try
        {
            return GetPropertyExpression(propertyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Не удалось сформировать выражение для параметра сортировки.");

            return GetPropertyExpression(_defaultPropertyBySorting);
        }
    }

    private static Expression<Func<AssemblyProductionOrders, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(AssemblyProductionOrders), "po");
        Expression propertyAccess = parameter;

        var parts = propertyName.Split(".");

        foreach (var part in parts)
        {
            var property = propertyAccess.Type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                ?? throw new ArgumentException($"Недопустимое имя свойства: {part} в пути {propertyName}");

            propertyAccess = Expression.Property(propertyAccess, property);
        }

        if (propertyAccess.Type != typeof(object))
        {
            propertyAccess = Expression.Convert(propertyAccess, typeof(object));
        }

        return Expression.Lambda<Func<AssemblyProductionOrders, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}