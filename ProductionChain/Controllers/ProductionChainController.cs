using Microsoft.AspNetCore.Mvc;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductionChainController : ControllerBase
{
    private readonly ILogger<ProductionChainController> _logger;

    public ProductionChainController(ILogger<ProductionChainController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionOrders()
    {
        return Ok();
    }
}
