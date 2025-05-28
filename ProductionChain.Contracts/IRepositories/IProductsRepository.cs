using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IRepositories;

public interface IProductsRepository : IRepository<Product>
{
    Task<ProductsPage> GetProductsAsync(GetQueryParameters queryParameters);
}
