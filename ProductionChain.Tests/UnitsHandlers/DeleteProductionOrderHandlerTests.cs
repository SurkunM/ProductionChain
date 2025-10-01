using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Delete;
using ProductionChain.Contracts.Exceptions;
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

    private readonly AssemblyProductionOrder _productionOrder;

    private readonly Order _order;

    public DeleteProductionOrderHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteProductionOrderHandler>>();

        _deleteProductionOrderHandler = new DeleteProductionOrderHandler(_uowMock.Object, _loggerMock.Object);

        var product = new Product
        {
            Id = 1,
            Name = "Product1",
            Model = "Model1"
        };

        _order = new Order
        {
            Id = 1,
            Customer = "Customer1",
            ProductId = product.Id,
            Product = product,
            StageType = ProgressStatusType.Pending
        };

        _productionOrder = new AssemblyProductionOrder
        {
            Id = 1,
            Order = _order,
            Product = product,
            StatusType = ProgressStatusType.Pending,
            CompletedProductsCount = 100,
            InProgressProductsCount = 0,
            TotalProductsCount = 100
        };
    }

    [Fact]
    public async Task Should_Successfully_ProcessAllSteps_And_SaveChanges()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrder.Id)).ReturnsAsync(_productionOrder);
        productionOrdersRepositoryMock.Setup(r => r.HasInProgressTasks(_productionOrder.Id)).Returns(false);
        productionOrdersRepositoryMock.Setup(r => r.IsCompleted(_productionOrder.Id)).Returns(true);
        productionOrdersRepositoryMock.Setup(r => r.Delete(It.IsAny<AssemblyProductionOrder>()));

        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.SetAvailableProductsCount(It.IsAny<int>(), _productionOrder.CompletedProductsCount));
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrder.Id)).ReturnsAsync(_order);
        ordersRepositoryMock.Setup(r => r.UpdateOrderStatus(It.IsAny<int>(), ProgressStatusType.Done));

        var assemblyWarehouseRepositoryMok = new Mock<IAssemblyProductionWarehouseRepository>();
        assemblyWarehouseRepositoryMok.Setup(r => r.AddWarehouseItems(It.IsAny<int>(), _productionOrder.CompletedProductsCount)).Returns(true);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionWarehouseRepository>()).Returns(assemblyWarehouseRepositoryMok.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await _deleteProductionOrderHandler.HandleAsync(_productionOrder.Id);

        productionOrdersRepositoryMock.Verify(r => r.Delete(_productionOrder), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrder.Id), Times.Once);
        productionOrdersRepositoryMock.Verify(r => r.HasInProgressTasks(_productionOrder.Id), Times.Once);

        ordersRepositoryMock.Verify(r => r.SetAvailableProductsCount(It.IsAny<int>(), _productionOrder.CompletedProductsCount), Times.Once);
        ordersRepositoryMock.Verify(r => r.UpdateOrderStatus(It.IsAny<int>(), ProgressStatusType.Done), Times.Once);

        assemblyWarehouseRepositoryMok.Verify(r => r.AddWarehouseItems(It.IsAny<int>(), _productionOrder.CompletedProductsCount), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_BeginTransaction_And_RollbackTransaction_WhenException()
    {
        var id = 2;

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Ошибка!"));

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await Assert.ThrowsAsync<Exception>(() => _deleteProductionOrderHandler.HandleAsync(id));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFountException_When_ProductionOrder_IsNull()
    {
        var id = 2;

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AssemblyProductionOrder)null!);

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _deleteProductionOrderHandler.HandleAsync(id));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_InvalidStateException_When_HasInProgressTasks_True()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();
        productionOrdersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrder.Id)).ReturnsAsync(_productionOrder);
        productionOrdersRepositoryMock.Setup(r => r.HasInProgressTasks(_productionOrder.Id)).Returns(true);

        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await Assert.ThrowsAsync<InvalidStateException>(() => _deleteProductionOrderHandler.HandleAsync(_productionOrder.Id));

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);
        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }
}
