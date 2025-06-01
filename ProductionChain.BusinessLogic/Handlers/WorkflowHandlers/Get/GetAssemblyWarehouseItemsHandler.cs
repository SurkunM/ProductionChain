using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.Contracts.ResponsesPages;

namespace ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;

public class GetAssemblyWarehouseItemsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAssemblyWarehouseItemsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<AssemblyWarehousePage> HandleAsync(GetQueryParameters queryParameters)
    {
        var assemblyWarehouseRepository = _unitOfWork.GetRepository<IAssemblyProductionWarehouseRepository>();

        return assemblyWarehouseRepository.GetAssemblyWarehouseItemsAsync(queryParameters);
    }
}
