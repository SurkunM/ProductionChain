using ProductionChain.Model.BasicEntities;

namespace ProductionChain.Contracts.Authentication;

public interface IJwtGenerationService
{
    Task<string> GenerateTokenAsync(Account account);
}
