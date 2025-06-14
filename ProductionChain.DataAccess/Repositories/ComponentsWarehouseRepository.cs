using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class ComponentsWarehouseRepository : BaseEfRepository<ComponentsWarehouse>, IComponentsWarehouseRepository
{
    private readonly ILogger<ComponentsWarehouseRepository> _logger;

    private const string _defaultPropertyBySorting = "Product.Name";

    public ComponentsWarehouseRepository(ProductionChainDbContext dbContext, ILogger<ComponentsWarehouseRepository> logger) : base(dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ComponentsWarehousePage> GetComponentsAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(cw => cw.Product.Name.Contains(queryParameters.Term)
                || cw.Product.Model.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var componentsWarehouseSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(cw => new ComponentsWarehouseResponse
            {
                Id = cw.Id,
                ProductName = cw.Product.Name,
                ProductModel = cw.Product.Model,
                ComponentType = cw.Type.ToString(),
                ProductsCount = cw.ComponentsCount
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = componentsWarehouseSortedResponse.Count;
        }

        return new ComponentsWarehousePage
        {
            ComponentsWarehouseItems = componentsWarehouseSortedResponse,
            TotalCount = totalCount
        };
    }

    public bool TakeComponentsByProductId(int productId, int takeComponentsCount)
    {
        var components = DbSet
            .Where(c => c.ProductId == productId)
            .ToArray();

        var minComponentsCount = components.Min(c => c.ComponentsCount);

        if (minComponentsCount < takeComponentsCount)
        {
            return false;
        }

        foreach (var component in components)
        {
            component.ComponentsCount -= takeComponentsCount;
        }

        return true;
    }

    private Expression<Func<ComponentsWarehouse, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<ComponentsWarehouse, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(ComponentsWarehouse), "cw");
        Expression propertyAccess = parameter;

        var parts = propertyName.Split('.');

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

        return Expression.Lambda<Func<ComponentsWarehouse, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}
