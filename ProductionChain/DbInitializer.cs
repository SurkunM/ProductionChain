using Microsoft.EntityFrameworkCore;
using ProductionChain.DataAccess;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.Enums;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain;

public class DbInitializer
{
    private readonly ProductionChainDbContext _dbContext;

    public DbInitializer(ProductionChainDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Initialize()
    {
        _dbContext.Database.Migrate();

        if (!_dbContext.Employees.Any() && !_dbContext.Products.Any() && !_dbContext.Orders.Any())
        {
            CreateBaseData();

            _dbContext.SaveChanges();
        }

        if (!_dbContext.AssemblyProductionWarehouse.Any() && !_dbContext.ComponentsWarehouse.Any())
        {
            CreateWarehousesData();

            _dbContext.SaveChanges();
        }
    }

    private void CreateBaseData()
    {
        var employee1 = CreateEmployee("Васильев", "Василий", "Васильевич", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);
        var employee2 = CreateEmployee("Александров", "Александр", "Александрович", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);

        var employee3 = CreateEmployee("Иванов", "Иван", "Иванович", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);
        var employee4 = CreateEmployee("Степанов", "Степан", "Степанович", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);

        var employee5 = CreateEmployee("Николаев", "Николай", "Николаевич", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);
        var employee6 = CreateEmployee("Петров", "Петр", "Петрович", EmployeePositionType.AssemblyREA, EmployeeStatusType.Available);

        _dbContext.Employees.AddRange(employee1, employee2, employee3, employee4, employee5, employee6);

        var product1 = CreateProduct("БП 1000", "0.01");
        var product2 = CreateProduct("БП 2000", "0.12");
        var product3 = CreateProduct("БП 3000", "0.03");
        var product4 = CreateProduct("ИПС 1000", "0.1");

        var order1 = CreateOrder("OOO T1", product1, 500);
        var order2 = CreateOrder("OOO T1", product2, 100);

        var order3 = CreateOrder("OAO Восток", product3, 200);
        var order4 = CreateOrder("OAO Восток", product4, 100);

        _dbContext.Orders.AddRange(order1, order2, order3, order4);
    }

    private void CreateWarehousesData()
    {
        var product1 = _dbContext.Products.FirstOrDefault(p => p.Id == 1) ?? throw new ArgumentNullException("Не найден продукт с id=1");
        var product2 = _dbContext.Products.FirstOrDefault(p => p.Id == 2) ?? throw new ArgumentNullException("Не найден продукт с id=2");
        var product3 = _dbContext.Products.FirstOrDefault(p => p.Id == 3) ?? throw new ArgumentNullException("Не найден продукт с id=3");
        var product4 = _dbContext.Products.FirstOrDefault(p => p.Id == 4) ?? throw new ArgumentNullException("Не найден продукт с id=4");

        var assemblyWarehouseItem1 = CreateAssemblyWarehouseItem(product1);
        var assemblyWarehouseItem2 = CreateAssemblyWarehouseItem(product2);
        var assemblyWarehouseItem3 = CreateAssemblyWarehouseItem(product3);
        var assemblyWarehouseItem4 = CreateAssemblyWarehouseItem(product4);

        _dbContext.AssemblyProductionWarehouse.AddRange(assemblyWarehouseItem1, assemblyWarehouseItem2, assemblyWarehouseItem3, assemblyWarehouseItem4);

        var component1 = CreateComponentsWarehouseItem(product1, ComponentType.CircuitBoard, 200);
        var component2 = CreateComponentsWarehouseItem(product1, ComponentType.DiodeBoard, 200);
        var component3 = CreateComponentsWarehouseItem(product1, ComponentType.Heatsink, 200);
        var component4 = CreateComponentsWarehouseItem(product1, ComponentType.Enclosure, 200);

        var component5 = CreateComponentsWarehouseItem(product2, ComponentType.CircuitBoard, 200);
        var component6 = CreateComponentsWarehouseItem(product2, ComponentType.DiodeBoard, 200);
        var component7 = CreateComponentsWarehouseItem(product2, ComponentType.Heatsink, 200);
        var component8 = CreateComponentsWarehouseItem(product2, ComponentType.Enclosure, 200);

        var component9 = CreateComponentsWarehouseItem(product3, ComponentType.CircuitBoard, 200);
        var component10 = CreateComponentsWarehouseItem(product3, ComponentType.DiodeBoard, 200);
        var component11 = CreateComponentsWarehouseItem(product3, ComponentType.Heatsink, 200);
        var component12 = CreateComponentsWarehouseItem(product3, ComponentType.Enclosure, 200);

        var component13 = CreateComponentsWarehouseItem(product4, ComponentType.CircuitBoard, 200);
        var component14 = CreateComponentsWarehouseItem(product4, ComponentType.DiodeBoard, 200);
        var component15 = CreateComponentsWarehouseItem(product4, ComponentType.Heatsink, 200);
        var component16 = CreateComponentsWarehouseItem(product4, ComponentType.Enclosure, 200);


        _dbContext.ComponentsWarehouse.AddRange(component1, component2, component3, component4, component5, component6,
            component7, component8, component9, component10, component11, component12, component13, component14, component15, component16);
    }

    private static Employee CreateEmployee(string lastName, string firstName, string middleName, EmployeePositionType positionType, EmployeeStatusType employeeStatus)
    {
        return new Employee()
        {
            LastName = lastName,
            FirstName = firstName,
            MiddleName = middleName,
            Position = positionType,
            Status = employeeStatus
        };
    }

    private static Product CreateProduct(string name, string model)
    {
        return new Product()
        {
            Name = name,
            Model = model,
        };
    }

    private static Order CreateOrder(string customer, Product product, int count)
    {
        return new Order()
        {
            Customer = customer,
            Product = product,
            OrderedProductsCount = count,
            AvailableProductsCount = 0,
            CreatedAt = DateTime.UtcNow,
            StageType = ProgressStatusType.Pending
        };
    }

    private static ComponentsWarehouse CreateComponentsWarehouseItem(Product product, ComponentType componentType, int count)
    {
        return new ComponentsWarehouse()
        {
            Product = product,
            Type = componentType,
            ComponentsCount = count
        };
    }

    private static AssemblyProductionWarehouse CreateAssemblyWarehouseItem(Product product)
    {
        return new AssemblyProductionWarehouse()
        {
            Product = product,
            ProductsCount = 0
        };
    }
}
