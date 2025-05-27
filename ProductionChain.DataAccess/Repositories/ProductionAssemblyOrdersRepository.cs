using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyOrdersRepository : BaseEfRepository<ProductionAssemblyOrders>, IProductionAssemblyOrdersRepository
{
    private readonly ILogger<ProductionAssemblyOrdersRepository> _logger;

    private const string _defaultPropertyBySorting = "Product.Name";

    public ProductionAssemblyOrdersRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyOrdersRepository> logger) : base(dbContext)
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
            .Select(po => new ProductionOrdersDto
            {
                Id = po.Id,
                ProductName = po.Product.Name,
                ProductModel = po.Product.Model,
                Count = po.Count,
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

    private Expression<Func<ProductionAssemblyOrders, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<ProductionAssemblyOrders, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(ProductionAssemblyOrders), "po");
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

        return Expression.Lambda<Func<ProductionAssemblyOrders, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}