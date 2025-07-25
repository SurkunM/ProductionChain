using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers;
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
    public async Task<IActionResult> GetEmployees([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка сотрудников переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var employees = await _getEmployeesHandler.HandleAsync(queryParameters);

            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка сотрудников не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var products = await _getProductsHandler.HandleAsync(queryParameters);

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка продукции не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка заказов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var orders = await _getOrdersHandler.HandleAsync(queryParameters);

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка заказов не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}
