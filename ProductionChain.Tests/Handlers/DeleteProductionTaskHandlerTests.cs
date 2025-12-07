using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.Workflow.Delete;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Handlers;

public class DeleteProductionTaskHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly DeleteProductionTaskHandler _deleteProductionTaskHandler;

    private readonly Mock<ILogger<DeleteProductionTaskHandler>> _loggerMock;

    private readonly ProductionTaskRequest _taskRequest;

    public DeleteProductionTaskHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProductionTaskHandler>>();

        _deleteProductionTaskHandler = new DeleteProductionTaskHandler(_uowMock.Object, _loggerMock.Object);

        _taskRequest = new ProductionTaskRequest
        {
            Id = 1,
            ProductionOrderId = 1,
            ProductId = 1,
            EmployeeId = 1,
            ProductsCount = 100
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

        var productionOrder = new AssemblyProductionOrder
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

        var task = new AssemblyProductionTask
        {
            ProductionOrder = productionOrder,
            Product = product,
            Employee = employee,
            ProductsCount = 100
        };

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();
        tasksRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.Id)).ReturnsAsync(task);
        tasksRepositoryMock.Setup(r => r.SetTaskEndTime(_taskRequest.Id));
        tasksRepositoryMock.Setup(r => r.Delete(It.IsAny<AssemblyProductionTask>()));

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.SubtractInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.AddCompletedCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId)).Returns(true);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, It.IsAny<EmployeeStatusType>())).Returns(true);

        var historiesRepositoryMock = new Mock<IProductionHistoryRepository>();
        historiesRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<ProductionHistory>()));

        var assemblyWarehouseRepositoryMok = new Mock<IAssemblyProductionWarehouseRepository>();
        assemblyWarehouseRepositoryMok.Setup(r => r.AddWarehouseItems(_taskRequest.ProductId, _taskRequest.ProductsCount)).Returns(true);

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductionHistoryRepository>()).Returns(historiesRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionWarehouseRepository>()).Returns(assemblyWarehouseRepositoryMok.Object);

        await _deleteProductionTaskHandler.HandleAsync(_taskRequest);

        productionOrdersRepositoryMock.Verify(r => r.SubtractInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.AddCompletedCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId), Times.Once);

        tasksRepositoryMock.Verify(r => r.SetTaskEndTime(_taskRequest.Id), Times.Once);
        tasksRepositoryMock.Verify(r => r.Delete(task), Times.Once);

        employeesRepositoryMock.Verify(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, It.IsAny<EmployeeStatusType>()), Times.Once);
        historiesRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<ProductionHistory>()), Times.Once);

        assemblyWarehouseRepositoryMok.Verify(r => r.AddWarehouseItems(It.IsAny<int>(), _taskRequest.ProductsCount), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }


    [Fact]
    public async Task ShouldBeginTransactionAndRollbackTransactionWhenException()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.SubtractInProgressCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.AddCompletedCount(_taskRequest.ProductionOrderId, _taskRequest.ProductsCount)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.UpdateProductionOrderStatus(_taskRequest.ProductionOrderId)).Returns(true);

        var employeesRepositoryMock = new Mock<IEmployeesRepository>();
        employeesRepositoryMock.Setup(r => r.UpdateEmployeeStatus(_taskRequest.EmployeeId, EmployeeStatusType.Available)).Returns(true);

        var tasksRepositoryMock = new Mock<IAssemblyProductionTasksRepository>();
        tasksRepositoryMock.Setup(r => r.GetByIdAsync(_taskRequest.Id)).ThrowsAsync(new Exception("Ошибка!"));

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionTasksRepository>()).Returns(tasksRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IEmployeesRepository>()).Returns(employeesRepositoryMock.Object);

        await Assert.ThrowsAsync<Exception>(() => _deleteProductionTaskHandler.HandleAsync(_taskRequest));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }
}
