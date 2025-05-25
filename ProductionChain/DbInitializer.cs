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
        _dbContext.Database.EnsureCreated();

        CreateData();

        _dbContext.SaveChanges();
    }

    private void CreateData()
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
        var product4 = CreateProduct("БС 1000", "0.4");

        var order1 = CreateOrder("OOO T1", product1, 500);
        var order2 = CreateOrder("OOO T1", product2, 1500);

        var order3 = CreateOrder("OAO Восток", product3, 2000);
        var order4 = CreateOrder("OAO Восток", product4, 100);

        _dbContext.Orders.AddRange(order1, order2, order3, order4);

        var component1 = CreateComponent(product1, ComponentType.CircuitBoard, 550);
        var component2 = CreateComponent(product1, ComponentType.DiodeBoard, 440);
        var component3 = CreateComponent(product1, ComponentType.Heatsink, 300);
        var component4 = CreateComponent(product1, ComponentType.Enclosure, 350);

        var component5 = CreateComponent(product2, ComponentType.CircuitBoard, 500);
        var component6 = CreateComponent(product2, ComponentType.DiodeBoard, 350);
        var component7 = CreateComponent(product2, ComponentType.Heatsink, 380);
        var component8 = CreateComponent(product2, ComponentType.Enclosure, 450);

        var component9 = CreateComponent(product3, ComponentType.CircuitBoard, 500);
        var component10 = CreateComponent(product3, ComponentType.DiodeBoard, 350);
        var component11 = CreateComponent(product3, ComponentType.Heatsink, 380);
        var component12 = CreateComponent(product3, ComponentType.Enclosure, 450);

        var component13 = CreateComponent(product4, ComponentType.DiodeBoard, 450);

        _dbContext.ComponentsWarehouse.AddRange(component1, component2, component3, component4, component5, component6,
            component7, component8, component9, component10, component11, component12, component13);
    }

    private static Employee CreateEmployee(string firstName, string lastName, string middleName, EmployeePositionType positionType, EmployeeStatusType employeeStatus)
    {
        return new Employee()
        {
            FirstName = firstName,
            LastName = lastName,
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
            Count = count,
            CreatedAt = DateTime.UtcNow,
            StageType = ProgressStatusType.Pending.ToString()
        };
    }

    private static ComponentsWarehouse CreateComponent(Product product, ComponentType componentType, int count)
    {
        return new ComponentsWarehouse()
        {
            Product = product,
            Type = componentType,
            Count = count
        };
    }
}
