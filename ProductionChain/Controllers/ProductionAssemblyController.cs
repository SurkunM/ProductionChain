using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;
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

    private readonly CreateProductionHistoryHandler _createProductionHistoryHandler;
    private readonly CreateProductionOrderHandler _createProductionOrderHandler;
    private readonly CreateProductionTaskHandler _createProductionTaskHandler;
    private readonly CreateProductInAssemblyWarehouseHandler _createProductToWarehouseHandler;

    private readonly UpdateProductionHistoryHandler _updateProductionHistoryHandler;
    private readonly UpdateProductionOrderHandler _updateProductionOrderHandler;
    private readonly UpdateProductionTaskHandler _updateProductionTaskHandler;
    private readonly UpdateAssemblyWarehouseHandler _updateProductToWarehouseHandler;

    private readonly DeleteProductionHistoryHandler _deleteProductionHistoryHandler;
    private readonly DeleteProductionOrderHandler _deleteProductionOrderHandler;
    private readonly DeleteProductionTaskHandler _deleteProductionTaskHandler;
    private readonly DeleteProductToAssemblyWarehouseHandler _deleteProductToWarehouseHandler;

    private readonly ILogger<ProductionAssemblyController> _logger;

    public ProductionAssemblyController(GetProductionHistoriesHandler getProductionHistoriesHandler, GetProductionOrdersHandler getProductionOrdersHandler,
        GetProductionTasksHandler getProductionTasksHandler, GetAssemblyWarehouseItemsHandler getAssemblyWarehouseItemsHandler,
        GetComponentsWarehouseItemsHandler getComponentsWarehouseItemsHandler,

        CreateProductionHistoryHandler createProductionHistoryHandler, CreateProductionOrderHandler createProductionOrderHandler,
        CreateProductionTaskHandler createProductionTaskHandler, CreateProductInAssemblyWarehouseHandler addProductToWarehouseHandler,

        UpdateProductionHistoryHandler updateProductionHistoryHandler, UpdateProductionOrderHandler updateProductionOrderHandler,
        UpdateProductionTaskHandler updateProductionTaskHandler, UpdateAssemblyWarehouseHandler updateProductToWarehouseHandler,

        DeleteProductionHistoryHandler deleteProductionHistoryHandler, DeleteProductionOrderHandler deleteProductionOrderHandler,
        DeleteProductionTaskHandler deleteProductionTaskHandler, DeleteProductToAssemblyWarehouseHandler deleteProductToWarehouseHandler,

        ILogger<ProductionAssemblyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _createProductionHistoryHandler = createProductionHistoryHandler ?? throw new ArgumentNullException(nameof(createProductionHistoryHandler));
        _createProductionOrderHandler = createProductionOrderHandler ?? throw new ArgumentNullException(nameof(createProductionOrderHandler));
        _createProductionTaskHandler = createProductionTaskHandler ?? throw new ArgumentNullException(nameof(createProductionTaskHandler));
        _createProductToWarehouseHandler = addProductToWarehouseHandler ?? throw new ArgumentNullException(nameof(addProductToWarehouseHandler));

        _getProductionHistoriesHandler = getProductionHistoriesHandler ?? throw new ArgumentNullException(nameof(getProductionHistoriesHandler));
        _getProductionOrdersHandler = getProductionOrdersHandler ?? throw new ArgumentNullException(nameof(getProductionOrdersHandler));
        _getProductionTasksHandler = getProductionTasksHandler ?? throw new ArgumentNullException(nameof(getProductionTasksHandler));
        _getAssemblyWarehouseItemsHandler = getAssemblyWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getAssemblyWarehouseItemsHandler));
        _getComponentsWarehouseItemsHandler = getComponentsWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getComponentsWarehouseItemsHandler));

        _updateProductionHistoryHandler = updateProductionHistoryHandler ?? throw new ArgumentNullException(nameof(updateProductionHistoryHandler));
        _updateProductionOrderHandler = updateProductionOrderHandler ?? throw new ArgumentNullException(nameof(updateProductionOrderHandler));
        _updateProductionTaskHandler = updateProductionTaskHandler ?? throw new ArgumentNullException(nameof(updateProductionTaskHandler));
        _updateProductToWarehouseHandler = updateProductToWarehouseHandler ?? throw new ArgumentNullException(nameof(updateProductToWarehouseHandler));

        _deleteProductionHistoryHandler = deleteProductionHistoryHandler ?? throw new ArgumentNullException(nameof(deleteProductionHistoryHandler));
        _deleteProductionOrderHandler = deleteProductionOrderHandler ?? throw new ArgumentNullException(nameof(deleteProductionOrderHandler));
        _deleteProductionTaskHandler = deleteProductionTaskHandler ?? throw new ArgumentNullException(nameof(deleteProductionTaskHandler));
        _deleteProductToWarehouseHandler = deleteProductToWarehouseHandler ?? throw new ArgumentNullException(nameof(deleteProductToWarehouseHandler));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionHistory([FromQuery] GetQueryParameters queryParameters)
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

    [HttpPost]
    public async Task<IActionResult> CreateProductionOrder([FromQuery] GetQueryParameters queryParameters)
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

    [HttpPost]
    public async Task<IActionResult> CreateProductionTask([FromQuery] GetQueryParameters queryParameters)
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
    public async Task<IActionResult> UpdateProductionHistory()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProductionOrder()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProductionTask()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAssemblyWarehouseItem()
    {
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionHistory([FromBody] int id)
    {
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionOrder([FromBody] int id)
    {
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionTask([FromBody] int id)
    {
        return BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductToWarehouse([FromBody] int id)
    {
        return BadRequest();
    }
}
