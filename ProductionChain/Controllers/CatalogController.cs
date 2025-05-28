using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.Contracts.Dto;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.QueryParameters;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CatalogController : ControllerBase
{
    private readonly GetEmployeesHandler _getEmployeesHandler;
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetOrdersHandler _getOrdersHandler;
    //private readonly GetEmployeesStatusesHandler _getEmployeesStatusesHandler;

    private readonly CreateOrderHandler _createOrderHandler;

    private readonly UpdateEmployeeStatusHandler _updateEmployeeStatusHandler;
    private readonly UpdateOrderHandler _updateOrderHandler;

    private readonly DeleteOrderHandler _deleteOrderHandler;

    private readonly ILogger<CatalogController> _logger;

    public CatalogController(GetEmployeesHandler getEmployeesHandler, 
        GetProductsHandler getProductsHandler, GetOrdersHandler getOrdersHandler,
        CreateOrderHandler createOrderHandler, UpdateEmployeeStatusHandler updateEmployeeStatusHandler,
        UpdateOrderHandler updateOrderHandler, DeleteOrderHandler deleteOrderHandler,
        ILogger<CatalogController> logger)
    {
        _createOrderHandler = createOrderHandler ?? throw new ArgumentNullException(nameof(createOrderHandler));

        _getEmployeesHandler = getEmployeesHandler ?? throw new ArgumentNullException(nameof(getEmployeesHandler));
       // _getEmployeesStatusesHandler = getEmployeesStatusesHandler ?? throw new ArgumentNullException(nameof(getEmployeesStatusesHandler));
        _getProductsHandler = getProductsHandler ?? throw new ArgumentNullException(nameof(getProductsHandler));
        _getOrdersHandler = getOrdersHandler ?? throw new ArgumentNullException(nameof(getOrdersHandler));

        _updateEmployeeStatusHandler = updateEmployeeStatusHandler ?? throw new ArgumentNullException(nameof(updateEmployeeStatusHandler));
        _updateOrderHandler = updateOrderHandler ?? throw new ArgumentNullException(nameof(updateOrderHandler));

        _deleteOrderHandler = deleteOrderHandler ?? throw new ArgumentNullException(nameof(deleteOrderHandler));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
    {
        if (orderRequest is null)
        {
            _logger.LogError("Ошибка! Объект orderRequest пуст.");

            return BadRequest("Объект \"Новый заказ\" пуст.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания заказа. {orderRequest}", orderRequest);
            return UnprocessableEntity(ModelState);
        }

        try
        {
            await _createOrderHandler.HandleAsync(orderRequest);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Заказ не создан.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
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

    [HttpPost]
    public async Task<IActionResult> UpdateOrder()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEmployeeStatus()
    {
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder([FromBody] int id)
    {
        return BadRequest();
    }
}
