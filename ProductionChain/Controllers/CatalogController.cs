using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Update;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CatalogController : ControllerBase
{
    private readonly GetEmployeesHandler _getEmployeesHandler;
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetOrdersHandler _getOrdersHandler;
    private readonly GetEmployeesStatusesHandler _getEmployeesStatusesHandler;

    private readonly CreateOrderHandler _createOrderHandler;

    private readonly UpdateEmployeeStatusHandler _updateEmployeeStatusHandler;
    private readonly UpdateOrderHandler _updateOrderHandler;

    private readonly DeleteOrderHandler _deleteOrderHandler;

    private readonly ILogger<CatalogController> _logger;

    public CatalogController(GetEmployeesHandler getEmployeesHandler, GetEmployeesStatusesHandler getEmployeesStatusesHandler,
        GetProductsHandler getProductsHandler, GetOrdersHandler getOrdersHandler,
        CreateOrderHandler createOrderHandler, UpdateEmployeeStatusHandler updateEmployeeStatusHandler,
        UpdateOrderHandler updateOrderHandler, DeleteOrderHandler deleteOrderHandler,
        ILogger<CatalogController> logger)
    {
        _createOrderHandler = createOrderHandler ?? throw new ArgumentNullException(nameof(createOrderHandler));

        _getEmployeesHandler = getEmployeesHandler ?? throw new ArgumentNullException(nameof(getEmployeesHandler));
        _getEmployeesStatusesHandler = getEmployeesStatusesHandler ?? throw new ArgumentNullException(nameof(getEmployeesStatusesHandler));
        _getProductsHandler = getProductsHandler ?? throw new ArgumentNullException(nameof(getProductsHandler));
        _getOrdersHandler = getOrdersHandler ?? throw new ArgumentNullException(nameof(getOrdersHandler));

        _updateEmployeeStatusHandler = updateEmployeeStatusHandler ?? throw new ArgumentNullException(nameof(updateEmployeeStatusHandler));
        _updateOrderHandler = updateOrderHandler ?? throw new ArgumentNullException(nameof(updateOrderHandler));

        _deleteOrderHandler = deleteOrderHandler ?? throw new ArgumentNullException(nameof(deleteOrderHandler));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeesStatuses()
    {
        return Ok();
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
