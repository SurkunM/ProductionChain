using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.QueryParameters;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductionAssemblyController : ControllerBase
{
    private readonly GetProductionHistoriesHandler _getProductionHistoriesHandler;
    private readonly GetProductionOrdersHandler _getProductionOrdersHandler;
    private readonly GetProductionTasksHandler _getProductionTasksHandler;
    private readonly GetAssemblyWarehouseItemsHandler _getAssemblyWarehouseItemsHandler;
    private readonly GetComponentsWarehouseItemsHandler _getComponentsWarehouseItemsHandler;

    private readonly CreateProductionOrderHandler _createProductionOrderHandler;
    private readonly CreateProductionTaskHandler _createProductionTaskHandler;

    private readonly DeleteProductionOrderHandler _deleteProductionOrderHandler;
    private readonly DeleteProductionTaskHandler _deleteProductionTaskHandler;

    private readonly ILogger<ProductionAssemblyController> _logger;

    public ProductionAssemblyController(
        GetProductionHistoriesHandler getProductionHistoriesHandler, GetProductionOrdersHandler getProductionOrdersHandler,
        GetProductionTasksHandler getProductionTasksHandler, GetAssemblyWarehouseItemsHandler getAssemblyWarehouseItemsHandler,
        GetComponentsWarehouseItemsHandler getComponentsWarehouseItemsHandler,

        CreateProductionOrderHandler createProductionOrderHandler, CreateProductionTaskHandler createProductionTaskHandler,

        DeleteProductionOrderHandler deleteProductionOrderHandler, DeleteProductionTaskHandler deleteProductionTaskHandler,

        ILogger<ProductionAssemblyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _createProductionOrderHandler = createProductionOrderHandler ?? throw new ArgumentNullException(nameof(createProductionOrderHandler));
        _createProductionTaskHandler = createProductionTaskHandler ?? throw new ArgumentNullException(nameof(createProductionTaskHandler));

        _getProductionHistoriesHandler = getProductionHistoriesHandler ?? throw new ArgumentNullException(nameof(getProductionHistoriesHandler));
        _getProductionOrdersHandler = getProductionOrdersHandler ?? throw new ArgumentNullException(nameof(getProductionOrdersHandler));
        _getProductionTasksHandler = getProductionTasksHandler ?? throw new ArgumentNullException(nameof(getProductionTasksHandler));
        _getAssemblyWarehouseItemsHandler = getAssemblyWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getAssemblyWarehouseItemsHandler));
        _getComponentsWarehouseItemsHandler = getComponentsWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getComponentsWarehouseItemsHandler));

        _deleteProductionOrderHandler = deleteProductionOrderHandler ?? throw new ArgumentNullException(nameof(deleteProductionOrderHandler));
        _deleteProductionTaskHandler = deleteProductionTaskHandler ?? throw new ArgumentNullException(nameof(deleteProductionTaskHandler));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionHistory([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка истории, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var histories = await _getProductionHistoriesHandler.HandleAsync(queryParameters);

            return Ok(histories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка истории не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionOrders([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка производственных заказов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var productionOrders = await _getProductionOrdersHandler.HandleAsync(queryParameters);

            return Ok(productionOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка производственных заказов не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionTasks([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка задач, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var tasks = await _getProductionTasksHandler.HandleAsync(queryParameters);

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка задач не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAssemblyWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции из склада ГП, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var items = await _getAssemblyWarehouseItemsHandler.HandleAsync(queryParameters);

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка продукции из склада ГП не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetComponentsWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции из склада компонентов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        try
        {
            var items = await _getComponentsWarehouseItemsHandler.HandleAsync(queryParameters);

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение списка продукции из склада компонентов не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionOrder(ProductionOrdersRequest productionOrdersRequest)
    {
        if (productionOrdersRequest is null)
        {
            _logger.LogError("Ошибка! Объект productionOrdersDto пуст.");

            return BadRequest("Передан пустой объект параметров.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания производственной задачи. {productionOrdersDto}", productionOrdersRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createProductionOrderHandler.HandleAsync(productionOrdersRequest);

            if (!isCreated)
            {
                _logger.LogError("Ошибка! Не удалось найти все сущности для создания производственной задачи.");//не верное сообщение

                return BadRequest("Переданы не корректные данные для создания производственной задачи.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Не удалось создать производственную задачу.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionTask(ProductionTaskRequest productionTaskRequest)
    {
        if (productionTaskRequest is null)
        {
            _logger.LogError("Ошибка! Объект \"productionTaskRequest\" пуст.");

            return BadRequest("Передан пустой объект параметров.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания задачи. {productionTaskRequest}", productionTaskRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createProductionTaskHandler.HandleAsync(productionTaskRequest);

            if (!isCreated)
            {
                _logger.LogError("Ошибка! Не удалось найти все сущности для создания задачи");

                return BadRequest("Переданы не корректные данные для создания новой задачи.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Задача не создана.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionOrder([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("Передано значение id меньше нуля. id={id}", id);

            return BadRequest("Передано не корректное значение.");
        }

        try
        {
            var isDeleted = await _deleteProductionOrderHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("Ошибка! Производственный заказ для удаления не существует. id={id}", id);

                return BadRequest("Производственный заказ для удаления не существует.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление производственного заказа не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionTask([FromBody] ProductionTaskRequest taskRequest)
    {
        if (taskRequest is null)
        {
            _logger.LogError("Не передан объект параметров.");

            return BadRequest("Не передан объект параметров.");
        }

        try
        {
            var isDeleted = await _deleteProductionTaskHandler.HandleAsync(taskRequest);

            if (!isDeleted)
            {
                _logger.LogError("Ошибка! Задача для удаления не существует.");

                return BadRequest("Задача для удаления не существует.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление задачи не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}
