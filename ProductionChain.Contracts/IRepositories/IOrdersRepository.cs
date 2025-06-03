using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;

namespace ProductionChain.Contracts.IRepositories;

public interface IOrdersRepository : IRepository<Order>
{
    Task<OrdersPage> GetOrdersAsync(GetQueryParameters queryParameters);

    Product? GetProductByOrderId(int orderId);

    bool UpdateOrderStatusByOrderId(int orderId, ProgressStatusType statusType);

    bool IsOrderPending(int orderId);
}
