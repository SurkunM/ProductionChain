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

public class AssemblyProductionWarehouseRepository : BaseEfRepository<AssemblyProductionWarehouse>, IAssemblyProductionWarehouseRepository
{
    private readonly ILogger<AssemblyProductionWarehouseRepository> _logger;

    private const string _defaultPropertyBySorting = "Product.Name";

    public AssemblyProductionWarehouseRepository(ProductionChainDbContext dbContext, ILogger<AssemblyProductionWarehouseRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<AssemblyWarehousePage> GetAssemblyWarehouseItemsAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(aw => aw.Product.Name.Contains(queryParameters.Term)
                || aw.Product.Model.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var assemblyWarehouseSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(aw => new AssemblyWarehouseItemResponse
            {
                Id = aw.Id,
                Name = aw.Product.Name,
                Model = aw.Product.Model,
                ProductsCount = aw.ProductsCount
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = assemblyWarehouseSortedResponse.Count;
        }

        return new AssemblyWarehousePage
        {
            AssemblyWarehouseItems = assemblyWarehouseSortedResponse,
            Total = totalCount
        };
    }

    private Expression<Func<AssemblyProductionWarehouse, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<AssemblyProductionWarehouse, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(AssemblyProductionWarehouse), "aw");
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

        return Expression.Lambda<Func<AssemblyProductionWarehouse, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}
