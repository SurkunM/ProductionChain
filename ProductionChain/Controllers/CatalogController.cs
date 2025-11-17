using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.Basic;
using ProductionChain.Contracts.QueryParameters;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CatalogController : ControllerBase
{
    private readonly GetEmployeesHandler _getEmployeesHandler;
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetOrdersHandler _getOrdersHandler;

    private readonly ILogger<CatalogController> _logger;

    public CatalogController(GetEmployeesHandler getEmployeesHandler,
        GetProductsHandler getProductsHandler, GetOrdersHandler getOrdersHandler,
        ILogger<CatalogController> logger)
    {
        _getEmployeesHandler = getEmployeesHandler ?? throw new ArgumentNullException(nameof(getEmployeesHandler));
        _getProductsHandler = getProductsHandler ?? throw new ArgumentNullException(nameof(getProductsHandler));
        _getOrdersHandler = getOrdersHandler ?? throw new ArgumentNullException(nameof(getOrdersHandler));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetEmployees([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка сотрудников переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var employees = await _getEmployeesHandler.HandleAsync(queryParameters);

        return Ok(employees);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProducts([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var products = await _getProductsHandler.HandleAsync(queryParameters);

        return Ok(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetOrders([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка заказов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var orders = await _getOrdersHandler.HandleAsync(queryParameters);

        return Ok(orders);
    }
}
