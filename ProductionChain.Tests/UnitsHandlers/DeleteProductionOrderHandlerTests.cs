using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.UnitsHandlers;

public class DeleteProductionOrderHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly DeleteProductionOrderHandler _deleteProductionOrderHandler;

    private readonly Mock<ILogger<DeleteProductionOrderHandler>> _loggerMock;

    public DeleteProductionOrderHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProductionOrderHandler>>();

        _deleteProductionOrderHandler = new DeleteProductionOrderHandler(_uowMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ShouldSuccessfullyProcessAllStepsAndSaveChanges()
    {
        var product = new Product
        {
            Id = 1,
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
            Id = 1,
            Order = order,
            Product = product,
            StatusType = ProgressStatusType.Pending,
            CompletedProductsCount = 100,
            InProgressProductsCount = 0,
            TotalProductsCount = 100
        };

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(productionOrder.Id)).ReturnsAsync(productionOrder);
        productionOrdersRepositoryMock.Setup(r => r.HasInProgressTasks(productionOrder.Id)).Returns(false);
        productionOrdersRepositoryMock.Setup(r => r.IsCompleted(productionOrder.Id)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.Delete(It.IsAny<AssemblyProductionOrder>()));

        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.SetAvailableProductsCount(It.IsAny<int>(), productionOrder.CompletedProductsCount));
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(productionOrder.Id)).ReturnsAsync(order);
        ordersRepositoryMock.Setup(r => r.UpdateOrderStatus(It.IsAny<int>(), ProgressStatusType.Done));

        var assemblyWarehouseRepositoryMok = new Mock<IAssemblyProductionWarehouseRepository>();
        assemblyWarehouseRepositoryMok.Setup(r => r.AddWarehouseItems(It.IsAny<int>(), productionOrder.CompletedProductsCount)).Returns(true);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionWarehouseRepository>()).Returns(assemblyWarehouseRepositoryMok.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await _deleteProductionOrderHandler.HandleAsync(productionOrder.Id);

        productionOrdersRepositoryMock.Verify(r => r.Delete(productionOrder), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(productionOrder.Id), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.HasInProgressTasks(productionOrder.Id), Times.Once);

        ordersRepositoryMock.Verify(r => r.SetAvailableProductsCount(It.IsAny<int>(), productionOrder.CompletedProductsCount), Times.Once);
        ordersRepositoryMock.Verify(r => r.UpdateOrderStatus(It.IsAny<int>(), ProgressStatusType.Done), Times.Once);

        assemblyWarehouseRepositoryMok.Verify(r => r.AddWarehouseItems(It.IsAny<int>(), productionOrder.CompletedProductsCount), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldBeginTransactionAndRollbackTransactionWhenException()
    {
        var id = 2;

        var productionOrderRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrderRepositoryMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Ошибка!"));

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrderRepositoryMock.Object);

        await Assert.ThrowsAsync<Exception>(() => _deleteProductionOrderHandler.HandleAsync(id));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }
}
