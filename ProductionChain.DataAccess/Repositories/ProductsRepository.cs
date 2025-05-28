using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.Dto;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace ProductionChain.DataAccess.Repositories;

public class ProductsRepository : BaseEfRepository<Product>, IProductsRepository
{
    private readonly ILogger<ProductsRepository> _logger;

    private const string _defaultPropertyBySorting = "Name";

    public ProductsRepository(ProductionChainDbContext dbContext, ILogger<ProductsRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<ProductsPage> GetProductsAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(e => e.Name.Contains(queryParameters.Term)
                || e.Model.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var productsDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(p => new ProductsDto
            {
                Id = p.Id,
                Name = p.Name,
                Model = p.Model
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = productsDtoSorted.Count;
        }

        return new ProductsPage
        {
            Products = productsDtoSorted,
            Total = totalCount
        };
    }

    private Expression<Func<Product, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<Product, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(Product), "p");

        var property = typeof(Product).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
          ?? throw new ArgumentException($"Недопустимое имя свойства: {propertyName}");

        var access = Expression.MakeMemberAccess(parameter, property);

        return Expression.Lambda<Func<Product, object>>(Expression.Convert(access, typeof(object)), parameter);
    }
}
