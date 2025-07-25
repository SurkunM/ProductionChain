using Microsoft.Extensions.Logging;
using Moq;
using ProductionChain.Contracts.QueryParameters;
using ProductionChain.DataAccess;
using ProductionChain.DataAccess.Repositories;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;
using Moq.EntityFrameworkCore;

namespace ProductionChain.Tests.Repositories.Units;

public class AssemblyProductionOrdersRepositoryTests1
{
    private readonly Mock<ProductionChainDbContext> _dbContextMock;
    private readonly Mock<ILogger<AssemblyProductionOrdersRepository>> _loggerMock;
    private readonly AssemblyProductionOrdersRepository _repository;

    public AssemblyProductionOrdersRepositoryTests1()
    {
        _dbContextMock = new Mock<ProductionChainDbContext>();
        _loggerMock = new Mock<ILogger<AssemblyProductionOrdersRepository>>();
        _repository = new AssemblyProductionOrdersRepository(_dbContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetProductionOrdersAsync_WithDefaultParameters_ReturnsPagedResult()
    {
        var userContextMock = new Mock<ProductionChainDbContext>();

        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };

        var testData = new List<AssemblyProductionOrder>
        {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
        };

        userContextMock.Setup(x => x.AssemblyProductionOrders).ReturnsDbSet(testData);// Вот так!


        var mockDbSet = testData.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Set<AssemblyProductionOrder>()).Returns(mockDbSet.Object);
        var queryParameters = new GetQueryParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetProductionOrdersAsync(queryParameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.ProductionOrders.Count);
        Assert.Equal("Product1", result.ProductionOrders[0].ProductName);
        Assert.Equal("Product2", result.ProductionOrders[1].ProductName);
    }

    [Fact]
    public async Task GetProductionOrdersAsync_WithTermFilter_ReturnsFilteredResults()
    {
        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };
        // Arrange
        var testData = new List<AssemblyProductionOrder>
    {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
    };

        var mockDbSet = testData.AsQueryable().BuildMockDbSet();
        _dbContextMock.Setup(x => x.Set<AssemblyProductionOrder>()).Returns(mockDbSet.Object);

        var queryParameters = new GetQueryParameters
        {
            Term = "iPhone"
        };

        // Act
        var result = await _repository.GetProductionOrdersAsync(queryParameters);

        // Assert
        Assert.Equal(2, result.ProductionOrders.Count);
        Assert.All(result.ProductionOrders, x => Assert.Contains("iPhone", x.ProductName));
    }

    [Fact]
    public async Task GetProductionOrdersAsync_WithSorting_ReturnsSortedResults()
    {
        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };
        // Arrange
        var testData = new List<AssemblyProductionOrder>
    {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
    };

        var mockDbSet = testData.AsQueryable().BuildMockDbSet();
        _dbContextMock.Setup(x => x.Set<AssemblyProductionOrder>()).Returns(mockDbSet.Object);

        var queryParameters = new GetQueryParameters
        {
            SortBy = "Product.Name",
            IsDescending = false
        };

        // Act
        var result = await _repository.GetProductionOrdersAsync(queryParameters);

        // Assert
        Assert.Equal("AProduct", result.ProductionOrders[0].ProductName);
        Assert.Equal("MProduct", result.ProductionOrders[1].ProductName);
        Assert.Equal("ZProduct", result.ProductionOrders[2].ProductName);
    }

    [Fact]
    public async Task GetProductionOrdersAsync_WithPagination_ReturnsPaginatedResults()
    {
        var product = new Product
        {
            Name = "Product1",
            Model = "Model1"
        };

        var order = new Order
        {
            Customer = "Customer1",
            Product = product,
            StageType = ProgressStatusType.InProgress
        };
        // Arrange
        var testData = new List<AssemblyProductionOrder>
    {
            new()
            {
                Id = 1,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 2,
                Product = product,
                Order = order,
                StatusType = ProgressStatusType.Pending
            },

            new()
            {
                Id = 3,
                Product = new Product{ Name = "Product2", Model = "Model2" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            }
    };
        // Arrange
        testData = Enumerable.Range(1, 15)
            .Select(i => new AssemblyProductionOrder
            {
                Id = i,
                Product = new Product { Name = $"Product{i}", Model = $"Model{i}" },
                Order = order,
                StatusType = ProgressStatusType.Pending
            })
            .ToList();

        var mockDbSet = testData.AsQueryable().BuildMockDbSet();
        _dbContextMock.Setup(x => x.Set<AssemblyProductionOrder>()).Returns(mockDbSet.Object);

        var queryParameters = new GetQueryParameters
        {
            PageNumber = 2,
            PageSize = 5
        };

        // Act
        var result = await _repository.GetProductionOrdersAsync(queryParameters);

        // Assert
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(5, result.ProductionOrders.Count);
        Assert.Equal("Product6", result.ProductionOrders[0].ProductName);
    }
}
