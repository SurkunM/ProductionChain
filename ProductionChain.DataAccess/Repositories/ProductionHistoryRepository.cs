﻿using Microsoft.EntityFrameworkCore;
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

public class ProductionHistoryRepository : BaseEfRepository<ProductionHistory>, IProductionHistoryRepository
{
    private readonly ILogger<ProductionHistoryRepository> _logger;

    private const string _defaultPropertyBySorting = "Product";

    public ProductionHistoryRepository(ProductionChainDbContext dbContext, ILogger<ProductionHistoryRepository> logger) : base(dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductionHistoriesPage> GetProductionHistoriesAsync(GetQueryParameters queryParameters)
    {
        var queryDbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            queryDbSet = queryDbSet.Where(h => h.Product.Contains(queryParameters.Term) || h.Employee.Contains(queryParameters.Term));
        }

        var orderByExpression = string.IsNullOrEmpty(queryParameters.SortBy)
            ? CreateSortExpression(_defaultPropertyBySorting)
            : CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? queryDbSet.OrderByDescending(orderByExpression)
            : queryDbSet.OrderBy(orderByExpression);

        var historiesSortedResponse = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select(h => new ProductionHistoryResponse
            {
                Id = h.Id,
                TaskId = h.ProductionTaskId,
                Employee = h.Employee,
                Product = h.Product,
                ProductsCount = h.ProductsCount,
                StartTime = h.StartTime,
                EndTime = h.EndTime
            })
            .ToListAsync();

        var totalCount = DbSet.Count();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = historiesSortedResponse.Count;
        }

        return new ProductionHistoriesPage
        {
            Histories = historiesSortedResponse,
            TotalCount = totalCount
        };
    }

    private Expression<Func<ProductionHistory, object>> CreateSortExpression(string propertyName)
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

    private static Expression<Func<ProductionHistory, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(ProductionHistory), "h");
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

        return Expression.Lambda<Func<ProductionHistory, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
    }
}