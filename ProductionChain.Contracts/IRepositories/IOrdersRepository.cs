using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IOrdersRepository
{
    Task<OrdersPage> GetOrdersAsync(GetQueryParameters queryParameters);
}
