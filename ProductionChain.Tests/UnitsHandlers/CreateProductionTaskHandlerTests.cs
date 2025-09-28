using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;


namespace ProductionChain.Tests.UnitsHandlers;

public class CreateProductionTaskHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly CreateProductionTaskHandler _createProductionTaskHandler;

    private readonly Mock<ILogger<CreateProductionTaskHandler>> _loggerMock;

    private readonly ProductionTaskRequest _taskRequest;

    private readonly Product _product;

    private readonly Order _order;

    private readonly AssemblyProductionOrder _productionOrder;

    private readonly Employee _employee;

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

        _product = new Product
        {
            Id = _taskRequest.ProductId,
            Name = "Product1",
            Model = "Model1"
        };

        _order = new Order
        {
            Id = 1,
            Customer = "Customer1",
            ProductId = _product.Id,
            Product = _product,
            StageType = ProgressStatusType.Pending
        };

        _productionOrder = new AssemblyProductionOrder
        {
            Order = _order,
            Product = _product,
            StatusType = ProgressStatusType.Pending,
            TotalProductsCount = 100
        };

        _employee = new Employee
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Position = EmployeePositionType.AssemblyREA,
            Status = EmployeeStatusType.Available
        };
    }

    [Fact]
    public async Task Should_Successfully_ProcessAllSteps_And_SaveChanges()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ReturnsAsync(_productionOrder);
        productionOrdersRepositoryMock.Setup(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.AddInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ReturnsAsync(_product);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ReturnsAsync(_employee);
        employeesRepositoryMock.Setup(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, EmployeeStatusType.Busy)).Returns(true);

        var componentsWarehouseRepositoryMock = new Mock<IComponentsWarehouseRepository>();
        componentsWarehouseRepositoryMock.Setup(r => r.TakeComponentsByProductId(_taskRequest.ProductId, _taskRequest.ProductsCount)).Returns(true);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();

        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IComponentsWarehouseRepository>()).Returns(componentsWarehouseRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);

        await _createProductionTaskHandler.HandleAsync(_taskRequest);

        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductionOrderId), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.AddInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount), Times.Once);

        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.EmployeeId), Times.Once);
        employeesRepositoryMock.Verify(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, EmployeeStatusType.Busy), Times.Once);

        componentsWarehouseRepositoryMock.Verify(r => r.TakeComponentsByProductId(_taskRequest.ProductId, _taskRequest.ProductsCount), Times.Once);
        productsRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductId), Times.Once);

        tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionTask>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_BeginTransaction_And_RollbackTransaction_When_Exception()
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

        await Assert.ThrowsAsync<Exception>(() => _createProductionTaskHandler.HandleAsync(_taskRequest));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_ProductionOrder_IsNull()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ReturnsAsync((AssemblyProductionOrder)null!);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ReturnsAsync(_product);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ReturnsAsync(_employee);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _createProductionTaskHandler.HandleAsync(_taskRequest));

        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductionOrderId), Times.Once);
        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.EmployeeId), Times.Once);
        productsRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductId), Times.Once);

        tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionTask>()), Times.Never);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_Product_IsNull()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ReturnsAsync(_productionOrder);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ReturnsAsync((Product)null!);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ReturnsAsync(_employee);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _createProductionTaskHandler.HandleAsync(_taskRequest));

        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductionOrderId), Times.Once);
        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.EmployeeId), Times.Once);
        productsRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductId), Times.Once);

        tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionTask>()), Times.Never);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_Employee_IsNull()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductionOrderId)).ReturnsAsync(_productionOrder);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.ProductId)).ReturnsAsync(_product);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.EmployeeId)).ReturnsAsync((Employee)null!);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _createProductionTaskHandler.HandleAsync(_taskRequest));

        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductionOrderId), Times.Once);
        employeesRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.EmployeeId), Times.Once);
        productsRepositoryMock.Verify(r => r.GetByIdAsync(_taskRequest.ProductId), Times.Once);

        tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionTask>()), Times.Never);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }
}
