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
        var employee1 = CreateEmployee("Васильев", "Василий", "Васильевич", EmployeePositionType.AssemblyREA);
        var employee2 = CreateEmployee("Александров", "Александр", "Александрович", EmployeePositionType.AssemblyREA);
        var employeeStatus1 = CreateEmployeeStatus(employee1, EmployeeStatusType.Available);
        var employeeStatus2 = CreateEmployeeStatus(employee2, EmployeeStatusType.Available);

        var employee3 = CreateEmployee("Иванов", "Иван", "Иванович", EmployeePositionType.AssemblyREA);
        var employee4 = CreateEmployee("Степанов", "Степан", "Степанович", EmployeePositionType.AssemblyREA);
        var employeeStatus3 = CreateEmployeeStatus(employee3, EmployeeStatusType.Available);
        var employeeStatus4 = CreateEmployeeStatus(employee4, EmployeeStatusType.Available);

        var employee5 = CreateEmployee("Николаев", "Николай", "Николаевич", EmployeePositionType.AssemblyREA);
        var employee6 = CreateEmployee("Петров", "Петр", "Петрович", EmployeePositionType.AssemblyREA);
        var employeeStatus5 = CreateEmployeeStatus(employee5, EmployeeStatusType.Available);
        var employeeStatus6 = CreateEmployeeStatus(employee6, EmployeeStatusType.Available);

        _dbContext.EmployeeStatuses.Add(employeeStatus1);
        _dbContext.EmployeeStatuses.Add(employeeStatus2);

        _dbContext.EmployeeStatuses.Add(employeeStatus3);
        _dbContext.EmployeeStatuses.Add(employeeStatus4);

        _dbContext.EmployeeStatuses.Add(employeeStatus5);
        _dbContext.EmployeeStatuses.Add(employeeStatus6);

        var product1 = CreateProduct("БП 1000", "0.01");
        var product2 = CreateProduct("БП 2000", "0.12");
        var product3 = CreateProduct("БП 3000", "0.03");
        var product4 = CreateProduct("БС 1000", "0.4");

        var order1 = CreateOrder("OOO T1", product1, 500);
        var order2 = CreateOrder("OOO T1", product2, 1500);

        var order3 = CreateOrder("OAO Восток", product3, 2000);
        var order4 = CreateOrder("OAO Восток", product4, 100);

        _dbContext.Orders.AddRange(order1, order2, order3, order4);

        var component1 = CreateComponent(product1, ComponentType.CircuitBoard.ToString(), 550);
        var component2 = CreateComponent(product1, ComponentType.DiodeBoard.ToString(), 440);
        var component3 = CreateComponent(product1, ComponentType.Heatsink.ToString(), 300);
        var component4 = CreateComponent(product1, ComponentType.Enclosure.ToString(), 350);

        var component5 = CreateComponent(product2, ComponentType.CircuitBoard.ToString(), 500);
        var component6 = CreateComponent(product2, ComponentType.DiodeBoard.ToString(), 350);
        var component7 = CreateComponent(product2, ComponentType.Heatsink.ToString(), 380);
        var component8 = CreateComponent(product2, ComponentType.Enclosure.ToString(), 450);

        var component9 = CreateComponent(product3, ComponentType.CircuitBoard.ToString(), 500);
        var component10 = CreateComponent(product3, ComponentType.DiodeBoard.ToString(), 350);
        var component11 = CreateComponent(product3, ComponentType.Heatsink.ToString(), 380);
        var component12 = CreateComponent(product3, ComponentType.Enclosure.ToString(), 450);

        var component13 = CreateComponent(product4, ComponentType.DiodeBoard.ToString(), 450);

        _dbContext.ComponentsWarehouse.AddRange(component1, component2, component3, component4, component5, component6, 
            component7, component8, component9, component10, component11, component12, component13);
    }

    private static Employee CreateEmployee(string firstName, string lastName, string middleName, EmployeePositionType positionType)
    {
        return new Employee()
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName,
            Position = positionType.ToString()
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

    private static EmployeeStatus CreateEmployeeStatus(Employee employee, EmployeeStatusType employeeStatus)
    {
        return new EmployeeStatus()
        {
            Employee = employee,
            StatusType = employeeStatus.ToString()
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

    private static ComponentsWarehouse CreateComponent(Product product, string name, int count)
    {
        return new ComponentsWarehouse()
        {
            Product = product,
            Name = name,
            Count = count
        };
    }
}
