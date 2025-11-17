using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.BusinessLogic.Handlers.Workflow.Create;
using ProductionChain.Contracts.Dto.Requests;
using ProductionChain.Contracts.Exceptions;
using ProductionChain.Contracts.IRepositories;
using ProductionChain.Contracts.IUnitOfWork;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.Tests.Handlers;

public class CreateProductionOrderHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;

    private readonly CreateProductionOrderHandler _createProductionOrderHandler;

    private readonly Mock<ILogger<CreateProductionOrderHandler>> _loggerMock;

    private readonly ProductionOrdersRequest _productionOrdersRequest;

    private readonly Order _order;

    private readonly Product _product;

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

        _product = new Product
        {
            Id = _productionOrdersRequest.ProductId,
            Name = "Product1",
            Model = "Model1"
        };

        _order = new Order
        {
            Id = _productionOrdersRequest.OrderId,
            Customer = "Customer1",
            ProductId = _product.Id,
            Product = _product,
            StageType = ProgressStatusType.Pending
        };
    }

    [Fact]
    public async Task Should_Successfully_ProcessAllSteps_And_SaveChanges()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();

        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync(_order);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync(_product);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await _createProductionOrderHandler.HandleAsync(_productionOrdersRequest);

        ordersRepositoryMock.Verify(r => r.IsOrderPending(_productionOrdersRequest.OrderId), Times.Once);
        ordersRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.OrderId), Times.Once);
        ordersRepositoryMock.Verify(r => r.UpdateOrderStatus(_productionOrdersRequest.OrderId, ProgressStatusType.InProgress), Times.Once);

        productsRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.ProductId), Times.Once);

        productionOrdersRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionOrder>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_BeginTransaction_And_RollbackTransaction_WhenException()
    {
        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ThrowsAsync(new Exception("Îøèáêà!"));
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ThrowsAsync(new Exception("Îøèáêà!"));

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);

        await Assert.ThrowsAsync<Exception>(() => _createProductionOrderHandler.HandleAsync(_productionOrdersRequest));

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_Order_IsNull()
    {
        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync((Order)null!); ;
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync(_product);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _createProductionOrderHandler.HandleAsync(_productionOrdersRequest));

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_NotFoundException_When_Product_IsNull()
    {
        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync(_order); ;
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync((Product)null!);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => _createProductionOrderHandler.HandleAsync(_productionOrdersRequest));

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_InvalidStateException_When_IsOrderPending_False()
    {
        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync(_order); ;
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(false);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync(_product);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);

        await Assert.ThrowsAsync<InvalidStateException>(() => _createProductionOrderHandler.HandleAsync(_productionOrdersRequest));

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_Successfully_IsOrderPending_True()
    {
        var productionOrdersRepositoryMock = new Mock<IAssemblyProductionOrdersRepository>();

        var ordersRepositoryMock = new Mock<IOrdersRepository>();
        ordersRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.OrderId)).ReturnsAsync(_order); ;
        ordersRepositoryMock.Setup(r => r.IsOrderPending(_productionOrdersRequest.OrderId)).Returns(true);

        var productsRepositoryMock = new Mock<IProductsRepository>();
        productsRepositoryMock.Setup(r => r.GetByIdAsync(_productionOrdersRequest.ProductId)).ReturnsAsync(_product);

        _uowMock.Setup(uow => uow.GetRepository<IOrdersRepository>()).Returns(ordersRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IProductsRepository>()).Returns(productsRepositoryMock.Object);
        _uowMock.Setup(uow => uow.GetRepository<IAssemblyProductionOrdersRepository>()).Returns(productionOrdersRepositoryMock.Object);

        await _createProductionOrderHandler.HandleAsync(_productionOrdersRequest);

        productsRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.ProductId), Times.Once);
        ordersRepositoryMock.Verify(r => r.GetByIdAsync(_productionOrdersRequest.OrderId), Times.Once);

        ordersRepositoryMock.Verify(r => r.IsOrderPending(_productionOrdersRequest.OrderId), Times.Once);
        ordersRepositoryMock.Verify(r => r.UpdateOrderStatus(_productionOrdersRequest.OrderId, ProgressStatusType.InProgress), Times.Once);

        productionOrdersRepositoryMock.Verify(r => r.CreateAsync(It.IsNotNull<AssemblyProductionOrder>()), Times.Once);

        _uowMock.Verify(uow => uow.SaveAsync(), Times.Once);
        _uowMock.Verify(uow => uow.BeginTransaction(), Times.Once);
        _uowMock.Verify(uow => uow.RollbackTransaction(), Times.Never);
    }
}