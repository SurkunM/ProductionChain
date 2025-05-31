using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class ProductionAssemblyHistoryRepository : BaseEfRepository<ProductionAssemblyHistory>, IProductionAssemblyHistoryRepository
{
    private readonly ILogger<ProductionAssemblyHistoryRepository> _logger;

    private const string _defaultPropertyBySorting = "AssemblyTask.Product.Name";

    public ProductionAssemblyHistoryRepository(ProductionChainDbContext dbContext, ILogger<ProductionAssemblyHistoryRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<ProductionHistoriesPage> GetProductionHistoriesAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(h => h.AssemblyTask.Employee.LastName.Contains(queryParameters.Term)
                || h.AssemblyTask.Product.Name.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var historiesDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(h => new HistoryDto
            {
                Id = h.Id,
                ProductName = h.AssemblyTask.Product.Name,
                EmployeeName = h.AssemblyTask.Employee.FirstName,
                StartTime = h.AssemblyTask.StartTime,
                EndTime = h.AssemblyTask.EndTime
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = historiesDtoSorted.Count;
        }

        return new ProductionHistoriesPage
        {
            Histories = historiesDtoSorted,
            Total = totalCount
        };
    }

    private Expression<Func<ProductionAssemblyHistory, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<ProductionAssemblyHistory, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(ProductionAssemblyHistory), "h");
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

        return Expression.Lambda<Func<ProductionAssemblyHistory, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}