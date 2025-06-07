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
    public async Task<IActionResult> GetProductionOrders([FromQuery] GetQueryParameters queryParameters)
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
    public async Task<IActionResult> GetProductionTasks([FromQuery] GetQueryParameters queryParameters)
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
                _logger.LogError("������! �� ������� ����� ��� �������� ��� �������� ���������������� ������.");//�� ������ ���������

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

    [HttpDelete]
    public async Task<IActionResult> DeleteProductionOrder([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("�������� �������� id ������ ����. id={id}", id);

            return BadRequest("�������� �� ���������� ��������.");
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
    public async Task<IActionResult> DeleteProductionTask([FromBody] ProductionTaskRequest taskRequest)
    {
        if (taskRequest is null)
        {
            _logger.LogError("�� ������� ������ ����������.");

            return BadRequest("�� ������� ������ ����������.");
        }

        try
        {
            var isDeleted = await _deleteProductionTaskHandler.HandleAsync(taskRequest);

            if (!isDeleted)
            {
                _logger.LogError("������! ������ ��� �������� �� ����������.");

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
}
