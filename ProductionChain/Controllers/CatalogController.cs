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

            return BadRequest("Передан пустой объект параметров.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания заказа. {orderRequest}", orderRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createOrderHandler.HandleAsync(orderRequest);

            if (!isCreated)
            {
                _logger.LogError("Ошибка! Не удалось найти сущность для создания заказа. ProductId={orderRequest.ProductId}", orderRequest.ProductId);

                return BadRequest("Переданы не корректные данные для создания заказа.");
            }

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
    public async Task<IActionResult> UpdateOrder(OrderRequest orderRequest)
    {
        if (orderRequest is null)
        {
            _logger.LogError("Ошибка! Объект orderRequest пуст.");

            return BadRequest("Передан пустой объект параметров.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Не корректно заполнены поля для изменения заказа.");

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isUpdated = await _updateOrderHandler.HandleAsync(orderRequest);

            if (!isUpdated)
            {
                _logger.LogError("Ошибка! Не удалось изменить заказ. Переданы не корректные параметры. ProductId={employeeRequest.Id}", orderRequest.ProductId);

                return BadRequest("Переданы не корректные данные для изменения заказа.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Заказ не изменен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEmployeeStatus(EmployeeStateRequest employeeRequest)
    {
        if (employeeRequest is null)
        {
            _logger.LogError("Ошибка! Объект employeeRequest пуст.");

            return BadRequest("Передан пустой объект параметров.");
        }

        try
        {
            var isUpdated = await _updateEmployeeStatusHandler.HandleAsync(employeeRequest);

            if (!isUpdated)
            {
                _logger.LogError("Ошибка! Не удалось изменить статус сотрудника. Переданы не корректные параметры. Id={employeeRequest.Id} StatusType={employeeRequest.StatusType}",
                    employeeRequest.Id, employeeRequest.StatusType);

                return BadRequest("Переданы не корректные данные для изменения статуса сотрудника.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Статус сотрудника не изменен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("Передано значение id меньше нуля. id={id}", id);

            BadRequest("Передано не корректное значение.");
        }

        try
        {
            var isDeleted = await _deleteOrderHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("Ошибка! Заказ для удаления не существует. id={id}", id);

                return BadRequest("Переданный заказ для удаления не существует.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление контакта не выполнено. id={id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}
