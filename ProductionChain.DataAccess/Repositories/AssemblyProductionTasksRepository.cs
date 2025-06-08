using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.WorkflowEntities;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class AssemblyProductionTasksRepository : BaseEfRepository<AssemblyProductionTask>, IAssemblyProductionTasksRepository
{
    private readonly ILogger<AssemblyProductionTasksRepository> _logger;

    private const string _defaultPropertyBySorting = "Product.Name";

    public AssemblyProductionTasksRepository(ProductionChainDbContext dbContext, ILogger<AssemblyProductionTasksRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<ProductionTasksPage> GetTasksAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(t => t.Product.Name.Contains(queryParameters.Term)
                || t.Product.Model.Contains(queryParameters.Term)
                || t.Employee.FirstName.Contains(queryParameters.Term)
                || t.Employee.LastName.Contains(queryParameters.Term)
                || (t.Employee.MiddleName != null && t.Employee.MiddleName.Contains(queryParameters.Term)));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var tasksSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(t => new ProductionTaskResponse
            {
                Id = t.Id,
                ProductionOrderId = t.ProductionOrderId,

                EmployeeId = t.EmployeeId,
                EmployeeLastName = t.Employee.LastName,
                EmployeeFirstName = t.Employee.FirstName,
                EmployeeMiddleName = t.Employee.MiddleName,

                ProductId = t.ProductId,
                ProductName = t.Product.Name,
                ProductModel = t.Product.Model,
                ProductsCount = t.ProductsCount,

                StartTime = t.StartTime,
                EndTime = t.EndTime
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = tasksSortedResponse.Count;
        }

        return new ProductionTasksPage
        {
            Tasks = tasksSortedResponse,
            TotalCount = totalCount
        };
    }

    public void SetTaskEndTime(int id)
    {
        var task = DbSet.FirstOrDefault(t => t.Id == id);

        if (task is not null)
        {
            task.EndTime = DateTime.Now;
        }
    }

    private Expression<Func<AssemblyProductionTask, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<AssemblyProductionTask, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(AssemblyProductionTask), "t");
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

        return Expression.Lambda<Func<AssemblyProductionTask, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}
