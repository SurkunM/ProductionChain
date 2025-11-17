using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.Basic;

public class GetProductsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ProductsPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var employeeRepository = _unitOfWork.GetRepository<IProductsRepository>();

        return employeeRepository.GetProductsAsync(queryParameters);
    }
}
