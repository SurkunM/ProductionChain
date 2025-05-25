using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductsRepository
{
    Task<ProductsPage> GetProductsAsync(GetQueryParameters queryParameters);
}
