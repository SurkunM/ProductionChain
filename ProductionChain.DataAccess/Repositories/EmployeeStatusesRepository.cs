using Microsoft.Extensions.Logging;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;

namespace ProductionChain.DataAccess.Repositories;

public class EmployeeStatusesRepository : BaseEfRepository<EmployeeStatus>, IEmployeeStatusesRepository
{
    private readonly ILogger<EmployeeStatusesRepository> _logger;

    public EmployeeStatusesRepository(ProductionChainDbContext dbContext, ILogger<EmployeeStatusesRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
