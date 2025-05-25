using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.Responses;

namespace ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;

public class GetOrdersHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<OrdersPage> HandleAsync(GetQueryParameters queryParameters)
    {
        var employeeRepository = _unitOfWork.GetRepository<IOrdersRepository>();

        return employeeRepository.GetOrdersAsync(queryParameters);
    }
}
