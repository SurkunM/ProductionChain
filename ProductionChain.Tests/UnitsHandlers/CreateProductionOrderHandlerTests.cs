using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.WorkflowHandlers.Create;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.UnitsHandlers;

public class CreateProductionOrderHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly CreateProductionOrderHandler _createProductionOrderHandler;

    private readonly Mock<ILogger<CreateProductionOrderHandler>> _loggerMock;

    private readonly ProductionOrdersRequest _productionOrdersRequest;

    public CreateProductionOrderHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateProductionOrderHandler>>();

        _createProductionOrderHandler = new CreateProductionOrderHandler(_uowMock.Object, _loggerMock.Object);

        _productionOrdersRequest = new ProductionOrdersRequest
        {
            OrderId = 1,
            ProductId = 1
        };
    }

    [Fact]
    public async Task ShouldSuccessfullyProcessAllStepsAndSaveChanges()
    {
        var product = new Product
        {
            Id = _productionOrdersRequest.ProductId,
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Id = _productionOrdersRequest.OrderId,
            Customer = "Customer1",
            ProductId = product.Id,
            Product = product,
            StageType = ProgressStatusType.Pending
        };

        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();

        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync(order);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync(product);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        var result = await _createProductionOrderHandler.HandleAsync(_productionOrdersRequest);

        Assert.True(result);

        ordersRepositoryMock.Verify(r => r.IsOrderPending(_productionOrdersRequest.OrderId), Times.Once);
        ordersRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.OrderId), Times.Once);
        ordersRepositoryMock.Verify(r => r.UpdateOrderStatus(_productionOrdersRequest.OrderId, ProgressStatusType.InProgress), Times.Once);

        productsRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.ProductId), Times.Once);

        productionOrdersRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionOrders>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ShouldBeginTransactionAndRollbackTransactionWhenException()
    {
        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ThrowsAsync(new Exception("Îøèáêà!"));
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ThrowsAsync(new Exception("Îøèáêà!"));

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);

        var result = await _createProductionOrderHandler.HandleAsync(_productionOrdersRequest);

        Assert.False(result);

        _uowMock.Verify(u => u.SaveAsync(), Times.Never);

        _uowMock.Verify(u => u.BeginTransaction(), Times.Once);
        _uowMock.Verify(u => u.RollbackTransaction(), Times.Once);
    }
}