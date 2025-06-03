using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto.Responses;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using System.Linq.Expressions;
using System.Reflection;

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
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(e => e.FirstName.Contains(queryParameters.Term)
                || e.LastName.Contains(queryParameters.Term)
                || (e.MiddleName != null && e.MiddleName.Contains(queryParameters.Term)));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var employeesSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(e => new EmployeeResponse
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                MiddleName = e.MiddleName,
                Position = e.Position.ToString(),
                Status = e.Status.ToString()
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = employeesSortedResponse.Count;
        }

        return new EmployeesPage
        {
            Employees = employeesSortedResponse,
            Total = totalCount
        };
    }

    public bool UpdateEmployeeStatusById(int employeeId, EmployeeStatusType statusType)
    {
        var employee = DbSet.FirstOrDefault(e => e.Id == employeeId);

        if (employee is null)
        {
            return false;
        }

        employee.Status = statusType;

        return true;
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
