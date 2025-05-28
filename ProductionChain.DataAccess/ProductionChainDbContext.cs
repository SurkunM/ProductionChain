using Microsoft.EntityFrameworkCore;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess;

public class ProductionChainDbContext : DbContext
{
    public virtual DbSet<Employee> Employees { get; set; }

    //public virtual DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ComponentsWarehouse> ComponentsWarehouse { get; set; }

    public virtual DbSet<ProductionAssemblyHistory> ProductionAssemblyHistory { get; set; }

    public virtual DbSet<ProductionAssemblyOrders> ProductionAssemblyOrders { get; set; }

    public virtual DbSet<ProductionAssemblyTask> ProductionAssemblyTasks { get; set; }

    public virtual DbSet<ProductionAssemblyWarehouse> ProductionAssemblyWarehouse { get; set; }

    public ProductionChainDbContext(DbContextOptions<ProductionChainDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(b =>
        {
            b.Property(b => b.FirstName)
                .HasMaxLength(50);

            b.Property(b => b.LastName)
                .HasMaxLength(50);

            b.Property(b => b.MiddleName)
                .HasMaxLength(50);

            b.Property(b => b.Position)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.Property(b => b.Customer)
                .HasMaxLength(100);

            b.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.Property(b => b.Name)
                .HasMaxLength(100);

            b.Property(b => b.Model)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<ComponentsWarehouse>(b =>
        {
            b.Property(b => b.Type)
                .HasMaxLength(100);

            b.HasOne(cw => cw.Product)
                .WithMany(p => p.ComponentsWarehouse)
                .HasForeignKey(cw => cw.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductionAssemblyHistory>(b =>
        {
            b.HasOne(ah => ah.AssemblyTask)
                .WithMany(at => at.AssemblyHistories)
                .HasForeignKey(ah => ah.AssemblyTaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ProductionAssemblyOrders>(b =>
        {
            b.HasOne(ao => ao.Product)
                .WithMany(p => p.ProductionOrders)
                .HasForeignKey(ao => ao.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(ao => ao.Order)
                .WithMany(o => o.AssemblyOrders)
                .HasForeignKey(ao => ao.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductionAssemblyTask>(b =>
        {
            b.HasOne(at => at.ProductionOrder)
                .WithMany(po => po.AssemblyTask)
                .HasForeignKey(at => at.ProductionOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(at => at.Employee)
                .WithMany(e => e.AssemblyTasks)
                .HasForeignKey(at => at.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(at => at.Product)
                .WithMany(p => p.AssemblyTask)
                .HasForeignKey(at => at.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductionAssemblyWarehouse>(b =>
        {
            b.HasOne(aw => aw.Product)
                .WithMany(p => p.AssemblyWarehouse)
                .HasForeignKey(aw => aw.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
