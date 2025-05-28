using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IOrdersRepository : IRepository<Order>
{
    Task<OrdersPage> GetOrdersAsync(GetQueryParameters queryParameters);

    Product GetOrderProduct(Order order);
}
