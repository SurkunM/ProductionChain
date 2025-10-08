using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Mapping;
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

    private readonly AddToTaskQueueHandler _addToTaskQueueHandler;
    private readonly GetTaskQueueHandler _getTaskQueueHandler;

    private readonly ILogger<ProductionAssemblyController> _logger;

    public ProductionAssemblyController(
        GetProductionHistoriesHandler getProductionHistoriesHandler, GetProductionOrdersHandler getProductionOrdersHandler,
        GetProductionTasksHandler getProductionTasksHandler, GetAssemblyWarehouseItemsHandler getAssemblyWarehouseItemsHandler,
        GetComponentsWarehouseItemsHandler getComponentsWarehouseItemsHandler, GetTaskQueueHandler getTaskQueueHandler,

        CreateProductionOrderHandler createProductionOrderHandler, CreateProductionTaskHandler createProductionTaskHandler,
        AddToTaskQueueHandler addToTaskQueueHandler,

        DeleteProductionOrderHandler deleteProductionOrderHandler, DeleteProductionTaskHandler deleteProductionTaskHandler,

        ILogger<ProductionAssemblyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _createProductionOrderHandler = createProductionOrderHandler ?? throw new ArgumentNullException(nameof(createProductionOrderHandler));
        _createProductionTaskHandler = createProductionTaskHandler ?? throw new ArgumentNullException(nameof(createProductionTaskHandler));

        _addToTaskQueueHandler = addToTaskQueueHandler ?? throw new ArgumentNullException(nameof(addToTaskQueueHandler));
        _getTaskQueueHandler = getTaskQueueHandler ?? throw new ArgumentNullException(nameof(getTaskQueueHandler));

        _getProductionHistoriesHandler = getProductionHistoriesHandler ?? throw new ArgumentNullException(nameof(getProductionHistoriesHandler));
        _getProductionOrdersHandler = getProductionOrdersHandler ?? throw new ArgumentNullException(nameof(getProductionOrdersHandler));
        _getProductionTasksHandler = getProductionTasksHandler ?? throw new ArgumentNullException(nameof(getProductionTasksHandler));
        _getAssemblyWarehouseItemsHandler = getAssemblyWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getAssemblyWarehouseItemsHandler));
        _getComponentsWarehouseItemsHandler = getComponentsWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getComponentsWarehouseItemsHandler));

        _deleteProductionOrderHandler = deleteProductionOrderHandler ?? throw new ArgumentNullException(nameof(deleteProductionOrderHandler));
        _deleteProductionTaskHandler = deleteProductionTaskHandler ?? throw new ArgumentNullException(nameof(deleteProductionTaskHandler));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProductionHistory([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка истории, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var histories = await _getProductionHistoriesHandler.HandleAsync(queryParameters);

        return Ok(histories);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetProductionOrders([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка производственных заказов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var productionOrders = await _getProductionOrdersHandler.HandleAsync(queryParameters);

        return Ok(productionOrders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<IActionResult> GetProductionTasks([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка задач, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var tasks = await _getProductionTasksHandler.HandleAsync(queryParameters, User.ToAccountContext());

        return Ok(tasks);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<IActionResult> GetAssemblyWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции из склада ГП, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var items = await _getAssemblyWarehouseItemsHandler.HandleAsync(queryParameters);

        return Ok(items);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<IActionResult> GetComponentsWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение списка продукции из склада компонентов, переданы не корректные параметры страницы.");

            return BadRequest(ModelState);
        }

        var items = await _getComponentsWarehouseItemsHandler.HandleAsync(queryParameters);

        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
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

        await _createProductionOrderHandler.HandleAsync(productionOrdersRequest);

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
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

        await _createProductionTaskHandler.HandleAsync(productionTaskRequest);

        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteProductionOrder([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("Передано значение id меньше нуля. id={id}", id);

            return BadRequest("Передано не корректное значение.");
        }

        await _deleteProductionOrderHandler.HandleAsync(id);

        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteProductionTask([FromBody] ProductionTaskRequest taskRequest)
    {
        if (taskRequest is null)
        {
            _logger.LogError("Не передан объект параметров.");

            return BadRequest("Не передан объект параметров.");
        }

        await _deleteProductionTaskHandler.HandleAsync(taskRequest);

        return NoContent();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> AddToTaskQueue([FromBody] int employeeId)
    {
        await _addToTaskQueueHandler.HandleAsync(employeeId);

        return Created();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult GetNextTaskQueueEmployee()
    {
        var nextEmployee = _getTaskQueueHandler.GetNextEmployeeHandle();

        return Ok(nextEmployee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult GetTaskQueue()
    {
        var taskQueueList = _getTaskQueueHandler.GetTaskQueueHandle();

        return Ok(taskQueueList);
    }
}
