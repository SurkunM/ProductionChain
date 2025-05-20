using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;

namespace ProductionChain.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductionChainController : ControllerBase
{
    private readonly GetProductionHistoriesHandler _getProductionHistoriesHandler;
    private readonly GetProductionOrdersHandler _getProductionOrdersHandler;
    private readonly GetProductionTasksHandler _getProductionTasksHandler;
    private readonly GetProductsToWarehouseHandler _getProductsToWarehouseHandler;

    private readonly CreateProductionHistoryHandler _createProductionHistoryHandler;
    private readonly CreateProductionOrderHandler _createProductionOrderHandler;
    private readonly CreateProductionTaskHandler _createProductionTaskHandler;
    private readonly AddProductToWarehouseHandler _addProductToWarehouseHandler;

    private readonly UpdateProductionHistoryHandler _updateProductionHistoryHandler;
    private readonly UpdateProductionOrderHandler _updateProductionOrderHandler;
    private readonly UpdateProductionTaskHandler _updateProductionTaskHandler;
    private readonly UpdateProductToWarehouseHandler _updateProductToWarehouseHandler;

    private readonly DeleteProductionHistoryHandler _deleteProductionHistoryHandler;
    private readonly DeleteProductionOrderHandler _deleteProductionOrderHandler;
    private readonly DeleteProductionTaskHandler _deleteProductionTaskHandler;
    private readonly DeleteProductToWarehouseHandler _deleteProductToWarehouseHandler;

    private readonly ILogger<ProductionChainController> _logger;

    public ProductionChainController(GetProductionHistoriesHandler getProductionHistoriesHandler, GetProductionOrdersHandler getProductionOrdersHandler,
        GetProductionTasksHandler getProductionTasksHandler, GetProductsToWarehouseHandler getProductsToWarehouseHandler,
        CreateProductionHistoryHandler createProductionHistoryHandler, CreateProductionOrderHandler createProductionOrderHandler,
        CreateProductionTaskHandler createProductionTaskHandler, AddProductToWarehouseHandler addProductToWarehouseHandler,
        UpdateProductionHistoryHandler updateProductionHistoryHandler, UpdateProductionOrderHandler updateProductionOrderHandler,
        UpdateProductionTaskHandler updateProductionTaskHandler, UpdateProductToWarehouseHandler updateProductToWarehouseHandler,
        DeleteProductionHistoryHandler deleteProductionHistoryHandler, DeleteProductionOrderHandler deleteProductionOrderHandler,
        DeleteProductionTaskHandler deleteProductionTaskHandler, DeleteProductToWarehouseHandler deleteProductToWarehouseHandler,
        ILogger<ProductionChainController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _createProductionHistoryHandler = createProductionHistoryHandler ?? throw new ArgumentNullException(nameof(createProductionHistoryHandler));
        _createProductionOrderHandler = createProductionOrderHandler ?? throw new ArgumentNullException(nameof(createProductionOrderHandler));
        _createProductionTaskHandler = createProductionTaskHandler ?? throw new ArgumentNullException(nameof(createProductionTaskHandler));
        _addProductToWarehouseHandler = addProductToWarehouseHandler ?? throw new ArgumentNullException(nameof(addProductToWarehouseHandler));

        _getProductionHistoriesHandler = getProductionHistoriesHandler ?? throw new ArgumentNullException(nameof(getProductionHistoriesHandler));
        _getProductionOrdersHandler = getProductionOrdersHandler ?? throw new ArgumentNullException(nameof(getProductionOrdersHandler));
        _getProductionTasksHandler = getProductionTasksHandler ?? throw new ArgumentNullException(nameof(getProductionTasksHandler));
        _getProductsToWarehouseHandler = getProductsToWarehouseHandler ?? throw new ArgumentNullException(nameof(getProductsToWarehouseHandler));

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
    public async Task<IActionResult> CreateProductionHistory()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionOrder()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionTask()
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse()
    {
        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionHistory()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionOrders()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionTasks()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsToWarehouse()
    {
        return Ok();
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
    public async Task<IActionResult> UpdateProductToWarehouse()
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
