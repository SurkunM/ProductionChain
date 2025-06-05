using Microsoft.EntityFrameworkCore;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess;

public class ProductionChainDbContext : DbContext
{
    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ComponentsWarehouse> ComponentsWarehouse { get; set; }

    public virtual DbSet<ProductionHistory> ProductionHistory { get; set; }

    public virtual DbSet<AssemblyProductionOrders> AssemblyProductionOrders { get; set; }

    public virtual DbSet<AssemblyProductionTask> AssemblyProductionTasks { get; set; }

    public virtual DbSet<AssemblyProductionWarehouse> AssemblyProductionWarehouse { get; set; }

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
                .OnDelete(DeleteBehavior.NoAction);
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
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssemblyProductionOrders>(b =>
        {
            b.HasOne(ao => ao.Product)
                .WithMany(p => p.ProductionOrders)
                .HasForeignKey(ao => ao.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(ao => ao.Order)
                .WithMany(o => o.ProductionOrders)
                .HasForeignKey(ao => ao.OrderId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssemblyProductionTask>(b =>
        {
            b.HasOne(at => at.ProductionOrder)
                .WithMany(po => po.ProductionTask)
                .HasForeignKey(at => at.ProductionOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(at => at.Employee)
                .WithMany(e => e.ProductionTasks)
                .HasForeignKey(at => at.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(at => at.Product)
                .WithMany(p => p.ProductionTask)
                .HasForeignKey(at => at.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssemblyProductionWarehouse>(b =>
        {
            b.HasOne(aw => aw.Product)
                .WithMany(p => p.AssemblyWarehouse)
                .HasForeignKey(aw => aw.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
