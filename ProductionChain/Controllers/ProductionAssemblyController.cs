using Microsoft.AspNetCore.Mvc;
using ProductionChain.BusinessLogic.Handlers.BasicHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Get;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Update;
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

    private readonly CreateProductionHistoryHandler _createProductionHistoryHandler;
    private readonly CreateProductionOrderHandler _createProductionOrderHandler;
    private readonly CreateProductionTaskHandler _createProductionTaskHandler;
    private readonly CreateAssemblyWarehouseItemHandler _createAssemblyWarehouseItemHandler;

    private readonly UpdateProductionOrderHandler _updateProductionOrderHandler;
    private readonly UpdateProductionTaskHandler _updateProductionTaskHandler;
    private readonly UpdateAssemblyWarehouseItemHandler _updateAssemblyWarehouseItemHandler;
    private readonly UpdateComponentsWarehouseItemsHandler _updateComponentsWarehouseItemsHandler;

    private readonly DeleteProductionHistoryHandler _deleteProductionHistoryHandler;
    private readonly DeleteProductionOrderHandler _deleteProductionOrderHandler;
    private readonly DeleteProductionTaskHandler _deleteProductionTaskHandler;
    private readonly DeleteAssemblyWarehouseItemHandler _deleteAssemblyWarehouseItemHandler;

    private readonly ILogger<ProductionAssemblyController> _logger;

    public ProductionAssemblyController(GetProductionHistoriesHandler getProductionHistoriesHandler, GetProductionOrdersHandler getProductionOrdersHandler,
        GetProductionTasksHandler getProductionTasksHandler, GetAssemblyWarehouseItemsHandler getAssemblyWarehouseItemsHandler,
        GetComponentsWarehouseItemsHandler getComponentsWarehouseItemsHandler,

        CreateProductionHistoryHandler createProductionHistoryHandler, CreateProductionOrderHandler createProductionOrderHandler,
        CreateProductionTaskHandler createProductionTaskHandler, CreateAssemblyWarehouseItemHandler createAssemblyWarehouseItemHandler,

        UpdateProductionOrderHandler updateProductionOrderHandler, UpdateProductionTaskHandler updateProductionTaskHandler,
        UpdateAssemblyWarehouseItemHandler updateAssemblyWarehouseItemHandler, UpdateComponentsWarehouseItemsHandler updateComponentsWarehouseItemsHandler,

        DeleteProductionHistoryHandler deleteProductionHistoryHandler, DeleteProductionOrderHandler deleteProductionOrderHandler,
        DeleteProductionTaskHandler deleteProductionTaskHandler, DeleteAssemblyWarehouseItemHandler deleteWarehouseItemHandler,

        ILogger<ProductionAssemblyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _createProductionHistoryHandler = createProductionHistoryHandler ?? throw new ArgumentNullException(nameof(createProductionHistoryHandler));
        _createProductionOrderHandler = createProductionOrderHandler ?? throw new ArgumentNullException(nameof(createProductionOrderHandler));
        _createProductionTaskHandler = createProductionTaskHandler ?? throw new ArgumentNullException(nameof(createProductionTaskHandler));
        _createAssemblyWarehouseItemHandler = createAssemblyWarehouseItemHandler ?? throw new ArgumentNullException(nameof(createAssemblyWarehouseItemHandler));

        _getProductionHistoriesHandler = getProductionHistoriesHandler ?? throw new ArgumentNullException(nameof(getProductionHistoriesHandler));
        _getProductionOrdersHandler = getProductionOrdersHandler ?? throw new ArgumentNullException(nameof(getProductionOrdersHandler));
        _getProductionTasksHandler = getProductionTasksHandler ?? throw new ArgumentNullException(nameof(getProductionTasksHandler));
        _getAssemblyWarehouseItemsHandler = getAssemblyWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getAssemblyWarehouseItemsHandler));
        _getComponentsWarehouseItemsHandler = getComponentsWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(getComponentsWarehouseItemsHandler));

        _updateProductionOrderHandler = updateProductionOrderHandler ?? throw new ArgumentNullException(nameof(updateProductionOrderHandler));
        _updateProductionTaskHandler = updateProductionTaskHandler ?? throw new ArgumentNullException(nameof(updateProductionTaskHandler));
        _updateAssemblyWarehouseItemHandler = updateAssemblyWarehouseItemHandler ?? throw new ArgumentNullException(nameof(updateAssemblyWarehouseItemHandler));
        _updateComponentsWarehouseItemsHandler = updateComponentsWarehouseItemsHandler ?? throw new ArgumentNullException(nameof(updateComponentsWarehouseItemsHandler));

        _deleteProductionHistoryHandler = deleteProductionHistoryHandler ?? throw new ArgumentNullException(nameof(deleteProductionHistoryHandler));
        _deleteProductionOrderHandler = deleteProductionOrderHandler ?? throw new ArgumentNullException(nameof(deleteProductionOrderHandler));
        _deleteProductionTaskHandler = deleteProductionTaskHandler ?? throw new ArgumentNullException(nameof(deleteProductionTaskHandler));
        _deleteAssemblyWarehouseItemHandler = deleteWarehouseItemHandler ?? throw new ArgumentNullException(nameof(deleteWarehouseItemHandler));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionHistory([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("������! ��� ������� �� ��������� ������ �������, �������� �� ���������� ��������� ��������.");

            return BadRequest(ModelState);
        }

        try
        {
            var histories = await _getProductionHistoriesHandler.HandleAsync(queryParameters);

            return Ok(histories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� ��������� ������ ������� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionOrder([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("������! ��� ������� �� ��������� ������ ���������������� �������, �������� �� ���������� ��������� ��������.");

            return BadRequest(ModelState);
        }

        try
        {
            var productionOrders = await _getProductionOrdersHandler.HandleAsync(queryParameters);

            return Ok(productionOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� ��������� ������ ���������������� ������� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProductionTask([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("������! ��� ������� �� ��������� ������ �����, �������� �� ���������� ��������� ��������.");

            return BadRequest(ModelState);
        }

        try
        {
            var tasks = await _getProductionTasksHandler.HandleAsync(queryParameters);

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� ��������� ������ ����� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAssemblyWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("������! ��� ������� �� ��������� ������ ��������� �� ������ ��, �������� �� ���������� ��������� ��������.");

            return BadRequest(ModelState);
        }

        try
        {
            var items = await _getAssemblyWarehouseItemsHandler.HandleAsync(queryParameters);

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� ��������� ������ ��������� �� ������ �� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetComponentsWarehouseItems([FromQuery] GetQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("������! ��� ������� �� ��������� ������ ��������� �� ������ �����������, �������� �� ���������� ��������� ��������.");

            return BadRequest(ModelState);
        }

        try
        {
            var items = await _getComponentsWarehouseItemsHandler.HandleAsync(queryParameters);

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� ��������� ������ ��������� �� ������ ����������� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionHistory(ProductionHistoryRequest productionHistoriesRequest)
    {
        if (productionHistoriesRequest is null)
        {
            _logger.LogError("������! ������ productionHistoriesRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("������! �������� �� ���������� ������ ��� �������� �������. {productionHistoriesRequest}", productionHistoriesRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createProductionHistoryHandler.HandleAsync(productionHistoriesRequest);

            if (!isCreated)
            {
                _logger.LogError("������! �� ������� ����� ��� �������� ��� �������� �������.");

                return BadRequest("�������� �� ���������� ������ ��� �������� �������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������� �� �������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionOrder(ProductionOrdersRequest productionOrdersRequest)
    {
        if (productionOrdersRequest is null)
        {
            _logger.LogError("������! ������ productionOrdersDto ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("������! �������� �� ���������� ������ ��� �������� ���������������� ������. {productionOrdersDto}", productionOrdersRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createProductionOrderHandler.HandleAsync(productionOrdersRequest);

            if (!isCreated)
            {
                _logger.LogError("������! �� ������� ����� ��� �������� ��� �������� ���������������� ������.");

                return BadRequest("�������� �� ���������� ������ ��� �������� ���������������� ������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! �� ������� ������� ���������������� ������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductionTask(ProductionTaskRequest productionTaskRequest)
    {
        if (productionTaskRequest is null)
        {
            _logger.LogError("������! ������ \"productionTaskRequest\" ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("������! �������� �� ���������� ������ ��� �������� ������. {productionTaskRequest}", productionTaskRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createProductionTaskHandler.HandleAsync(productionTaskRequest);

            if (!isCreated)
            {
                _logger.LogError("������! �� ������� ����� ��� �������� ��� �������� ������");

                return BadRequest("�������� �� ���������� ������ ��� �������� ����� ������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������ �� �������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssemblyWarehouseItems(AssemblyWarehouseRequest warehouseItemRequest)
    {
        if (warehouseItemRequest is null)
        {
            _logger.LogError("������! ������ warehouseItemRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("������! �������� �� ���������� ������ ��� �������� ������� � ������ ������. {warehouseItemRequest}", warehouseItemRequest);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createAssemblyWarehouseItemHandler.HandleAsync(warehouseItemRequest);

            if (!isCreated)
            {
                _logger.LogError("������! �� ������� ����� ��� �������� ��� �������� ������� � ������ ������.");

                return BadRequest("�������� �� ���������� ������ ��� �������� ������� � ������ ������..");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ������� � ������ ������ �� �������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProductionOrder(ProductionOrdersRequest productionOrdersRequest)
    {
        if (productionOrdersRequest is null)
        {
            _logger.LogError("������! ������ productionOrdersRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        try
        {
            var isUpdated = await _updateProductionOrderHandler.HandleAsync(productionOrdersRequest);

            if (!isUpdated)
            {
                _logger.LogError("������! �� ������� ������ ��������� � ���������������� �����.");

                return BadRequest("�������� �� ���������� ������ ��� ��������� ����������������� ������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ���������������� ����� �� �������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProductionTask(ProductionTaskRequest productionTaskRequest)
    {
        if (productionTaskRequest is null)
        {
            _logger.LogError("������! ������ productionTaskRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        try
        {
            var isUpdated = await _updateProductionTaskHandler.HandleAsync(productionTaskRequest);

            if (!isUpdated)
            {
                _logger.LogError("������! �� ������� ������ ��������� � ���������������� ������.");

                return BadRequest("�������� �� ���������� ������ ��� ��������� ���������������� ������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ���������������� ������ �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAssemblyWarehouseItem(AssemblyWarehouseRequest assemblyWarehouseRequest)
    {
        if (assemblyWarehouseRequest is null)
        {
            _logger.LogError("������! ������ assemblyWarehouseRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        try
        {
            var isUpdated = await _updateAssemblyWarehouseItemHandler.HandleAsync(assemblyWarehouseRequest);

            if (!isUpdated)
            {
                _logger.LogError("������! �� ������� ������ ��������� � ����� ������.");

                return BadRequest("�������� �� ���������� ������ ��� ��������� ������� � ������ ������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ���������� ��������� � ������ ������ �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateComponentsWarehouseItem(ComponentsWarehouseRequest componentsWarehouseRequest)
    {
        if (componentsWarehouseRequest is null)
        {
            _logger.LogError("������! ������ componentsWarehouseRequest ����.");

            return BadRequest("������� ������ ������ ����������.");
        }

        try
        {
            var isUpdated = await _updateComponentsWarehouseItemsHandler.HandleAsync(componentsWarehouseRequest);

            if (!isUpdated)
            {
                _logger.LogError("������! �� ������� ������ ��������� � ����� �����������.");

                return BadRequest("�������� �� ���������� ������ ��� ��������� ������� � ������ �����������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! ���������� ��������� � ������ ����������� �� ��������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionHistory([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("�������� �������� id ������ ����. id={id}", id);

            BadRequest("�������� �� ���������� ��������.");
        }

        try
        {
            var isDeleted = await _deleteProductionHistoryHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("������! ������� ��� �������� �� ����������. id={id}", id);

                return BadRequest("������� ��� �������� �� ����������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! �������� ������� �� ���������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionOrder([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("�������� �������� id ������ ����. id={id}", id);

            BadRequest("�������� �� ���������� ��������.");
        }

        try
        {
            var isDeleted = await _deleteProductionOrderHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("������! ���������������� ����� ��� �������� �� ����������. id={id}", id);

                return BadRequest("���������������� ����� ��� �������� �� ����������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! �������� ����������������� ������ �� ���������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionTask([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("�������� �������� id ������ ����. id={id}", id);

            BadRequest("�������� �� ���������� ��������.");
        }

        try
        {
            var isDeleted = await _deleteProductionTaskHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("������! ������ ��� �������� �� ����������. id={id}", id);

                return BadRequest("������ ��� �������� �� ����������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! �������� ������ �� ���������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAssemblyWarehouseItem([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("�������� �������� id ������ ����. id={id}", id);

            BadRequest("�������� �� ���������� ��������.");
        }

        try
        {
            var isDeleted = await _deleteAssemblyWarehouseItemHandler.HandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("������! ������� ��� �������� � ������ ������ �� ����������. id={id}", id);

                return BadRequest("������� ��� �������� � ������ ������ �� ����������.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "������! �������� ������� � ������ ������ �� ���������.");

            return StatusCode(StatusCodes.Status500InternalServerError, "������ �������.");
        }
    }
}
