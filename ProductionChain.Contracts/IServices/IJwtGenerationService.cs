using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.IServices;

public interface IJwtGenerationService
{
    Task<string> GenerateTokenAsync(Account account);
}
