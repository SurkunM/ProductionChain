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

public class OrdersRepository : BaseEfRepository<Order>, IOrdersRepository
{
    private readonly ILogger<OrdersRepository> _logger;

    private const string _defaultPropertyBySorting = "Customer";

    public OrdersRepository(ProductionChainDbContext dbContext, ILogger<OrdersRepository> logger) : base(dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrdersPage> GetOrdersAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(o => o.Customer.Contains(queryParameters.Term)
                || o.Product.Name.Contains(queryParameters.Term)
                || o.Product.Model.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var ordersSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                Customer = o.Customer,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                ProductModel = o.Product.Model,
                OrderedProductsCount = o.OrderedProductsCount,
                AvailableProductsCount = o.AvailableProductsCount,
                Status = o.StageType.ToString(),
                CreateAt = o.CreatedAt
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = ordersSortedResponse.Count;
        }

        return new OrdersPage
        {
            Orders = ordersSortedResponse,
            TotalCount = totalCount
        };
    }

    public bool IsOrderPending(int orderId)
    {
        var order = DbSet.First(o => o.Id == orderId);

        return order.StageType == ProgressStatusType.Pending;
    }

    public bool UpdateOrderStatus(int orderId, ProgressStatusType statusType)
    {
        var order = DbSet.FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            return false;
        }

        order.StageType = statusType;

        return true;
    }

    public bool SetAvailableProductsCount(int orderId, int productsCount)
    {
        var order = DbSet.FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            return false;
        }

        order.AvailableProductsCount += productsCount;

        return true;
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
