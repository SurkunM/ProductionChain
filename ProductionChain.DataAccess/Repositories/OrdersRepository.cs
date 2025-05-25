using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class OrdersRepository : BaseEfRepository<Order>, IOrdersRepository
{
    private readonly ILogger<OrdersRepository> _logger;

    private const string _defaultPropertyBySorting = "Customer";

    public OrdersRepository(ProductionChainDbContext dbContext, ILogger<OrdersRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<OrdersPage> GetOrdersAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim().ToUpper();
            queryDbSet = queryDbSet.Where(o => o.Customer.ToUpper().Contains(queryParameters.Term)
                || o.Product.Name.ToUpper().Contains(queryParameters.Term)
                || o.Product.Model.ToUpper().Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var ordersDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select((c) => new OrdersDto
            {
                Id = c.Id,
                Customer = c.Customer,
                ProductName = c.Product.Name,
                ProductModel = c.Product.Model,
                Count = c.Count,
                Status = c.StageType.ToString(),
                CreateAt = c.CreatedAt
            })
            .ToListAsync();

        for (int i = 0; i < ordersDtoSorted.Count; i++)
        {
            ordersDtoSorted[i].Index = (queryParameters.PageNumber - 1) * queryParameters.PageSize + i + 1; ;
        }

        var totalCount = await DbSet.CountAsync();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = ordersDtoSorted.Count;
        }

        return new OrdersPage
        {
            Orders = ordersDtoSorted,
            Total = totalCount
        };
    }

    private Expression<Func<Order, object>> CreateSortExpression(string propertyName)
    {
        try
        {
            return GetPropertyExpression(propertyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Не удалось сформировать выражение для параметра сортировки. Выполнена сортировка по умолчанию");

            return GetPropertyExpression(_defaultPropertyBySorting);
        }
    }

    private static Expression<Func<Order, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(Order), "o");

        var property = typeof(Order).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
          ?? throw new ArgumentException($"Недопустимое имя свойства: {propertyName}");

        var access = Expression.MakeMemberAccess(parameter, property);

        return Expression.Lambda<Func<Order, object>>(Expression.Convert(access, typeof(object)), parameter);
    }
}
