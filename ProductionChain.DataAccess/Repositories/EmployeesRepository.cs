using ProductionChain.DataAccess.Repositories.BaseAbstractions;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Contracts.IRepositories;
using Microsoft.Extensions.Logging;

namespace ProductionChain.DataAccess.Repositories;

public class EmployeesRepository : BaseEfRepository<Employee>, IEmployeesRepository
{
    private readonly ILogger<EmployeesRepository> _logger;

    public EmployeesRepository(ProductionChainDbContext dbContext, ILogger<EmployeesRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
}
