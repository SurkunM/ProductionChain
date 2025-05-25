using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Contracts.IRepositories;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Responses;
using ProductionChain.Contracts.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using ProductionChain.Contracts.Dto;

namespace ProductionChain.DataAccess.Repositories;

public class EmployeesRepository : BaseEfRepository<Employee>, IEmployeesRepository
{
    private readonly ILogger<EmployeesRepository> _logger;

    private const string _defaultPropertyBySorting = "LastName";

    public EmployeesRepository(ProductionChainDbContext dbContext, ILogger<EmployeesRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<EmployeesPage> GetEmployeesAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim().ToUpper();
            queryDbSet = queryDbSet.Where(e => e.FirstName.ToUpper().Contains(queryParameters.Term)
                || e.LastName.ToUpper().Contains(queryParameters.Term)
                || (e.MiddleName != null && e.MiddleName.ToUpper().Contains(queryParameters.Term)));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy) 
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var employeesDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select((c) => new EmployeeDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                Position = c.Position.ToString(),
                Status = c.Status.ToString()
            })
            .ToListAsync();

        for (int i = 0; i < employeesDtoSorted.Count; i++)
        {
            employeesDtoSorted[i].Index = (queryParameters.PageNumber - 1) * queryParameters.PageSize + i + 1; ;
        }

        var totalCount = await DbSet.CountAsync();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = employeesDtoSorted.Count;
        }

        return new EmployeesPage
        {
            Employees = employeesDtoSorted,
            Total = totalCount
        };
    }

    private Expression<Func<Employee, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<Employee, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(Employee), "e");

        var property = typeof(Employee).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
          ?? throw new ArgumentException($"Недопустимое имя свойства: {propertyName}");

        var access = Expression.MakeMemberAccess(parameter, property);

        return Expression.Lambda<Func<Employee, object>>(Expression.Convert(access, typeof(object)), parameter);
    }
}
