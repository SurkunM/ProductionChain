using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;


namespace ProductionChain.Tests.Handlers;

public class CreateProductionTaskHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly CreateProductionTaskHandler _createProductionTaskHandler;

    private readonly Mock<ILogger<CreateProductionTaskHandler>> _loggerMock;

    private readonly ProductionTaskRequest _taskRequest;

    public CreateProductionTaskHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateProductionTaskHandler>>();

        _createProductionTaskHandler = new CreateProductionTaskHandler(_uowMock.Object, _loggerMock.Object);

        _taskRequest = new ProductionTaskRequest
        {
            Id = 1,
            ProductionOrderId = 1,
            ProductId = 1,
            EmployeeId = 1,
            ProductsCount = 10
        };
    }

    [Fact]

    public async Task ShouldSuccessfullyProcessAllStepsAndSaveChanges()
    {
        var product = new Product
        {
            Id = _taskRequest.ProductId,
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Id = 1,
            Customer = "Customer1",
            ProductId = product.Id,
            Product = product,
            StageType = ProgressStatusType.Pending
        };

        var productionOrder = new AssemblyProductionOrders
        {
            Order = order,
            Product = product,
            StatusType = ProgressStatusType.Pending,
            TotalProductsCount = 100
        };

        var employee = new Employee
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Available
        };

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ReturnsAsync(productionOrder);
        productionOrdersRepositoryMock.Setup(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.AddInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ReturnsAsync(product);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ReturnsAsync(employee);
        employeesRepositoryMock.Setup(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, EmployeeStatusType.Busy)).Returns(true);

        var componentsWarehouseRepositoryMock = new Mock<IComponentsWarehouseRepository>();
        componentsWarehouseRepositoryMock.Setup(r => r.TakeComponentsByProductId(_taskRequest.ProductId, _taskRequest.ProductsCount)).Returns(true);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();

        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IComponentsWarehouseRepository>()).Returns(componentsWarehouseRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);


        var result = await _createProductionTaskHandler.HandleAsync(_taskRequest);

        Assert.True(result);

        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductionOrderId), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.AddInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount), Times.Once);

        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.EmployeeId), Times.Once);
        employeesRepositoryMock.Verify(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, EmployeeStatusType.Busy), Times.Once);

        componentsWarehouseRepositoryMock.Verify(r => r.TakeComponentsByProductId(_taskRequest.ProductId, _taskRequest.ProductsCount), Times.Once);
        productsRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductId), Times.Once);

        tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionTask>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldBeginTransactionAndRollbackTransactionWhenException()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ThrowsAsync(new Exception("Ошибка!"));

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ThrowsAsync(new Exception("Ошибка!"));

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ThrowsAsync(new Exception("Ошибка!"));

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);

        var result = await _createProductionTaskHandler.HandleAsync(_taskRequest);

        Assert.False(result);

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);

        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }
}
