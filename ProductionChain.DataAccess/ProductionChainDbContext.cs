using Microsoft.EntityFrameworkCore;
using ProductionChain.Model.BasicEntities;
using ProductionChain.Model.WorkflowEntities;

namespace ProductionChain.DataAccess;

public class ProductionChainDbContext : DbContext
{
    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductionStage> ProductionStages { get; set; }

    public virtual DbSet<Warehouse> Warehouse { get; set; }

    public virtual DbSet<ProductionHistory> ProductionHistory { get; set; }

    public virtual DbSet<ProductionOrders> ProductionOrders { get; set; }

    public virtual DbSet<ProductionTask> ProductionTasks { get; set; }

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
        });

        modelBuilder.Entity<EmployeeStatus>(b =>
        {
            b.HasOne(es => es.Employee)
                .WithMany()
                .HasForeignKey(es => es.EmployeeId);
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.Property(b => b.Customer)
                .HasMaxLength(100);

            b.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.Property(b => b.Name)
                .HasMaxLength(100);

            b.Property(b => b.Model)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<ProductionStage>(b =>
        {
            b.Property(b => b.Name)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Warehouse>(b =>
        {
            b.HasOne(w => w.Product)
                .WithMany(p => p.Warehouses)
                .HasForeignKey(w => w.ProductId);
        });

        modelBuilder.Entity<ProductionHistory>(b =>
        {
            b.HasOne(ph => ph.ProductionTask)
                .WithMany(pt => pt.Histories)
                .HasForeignKey(ph => ph.ProductionTaskId);
        });

        modelBuilder.Entity<ProductionOrders>(b =>
        {
            b.HasOne(po => po.Product)
                .WithMany(p => p.ProductionOrders)
                .HasForeignKey(po => po.ProductId);
        });

        modelBuilder.Entity<ProductionTask>(b =>
        {
            b.HasOne(pt => pt.ProductionOrders)
                .WithMany(po => po.ProductionTask)
                .HasForeignKey(pt => pt.ProductionOrdersId);

            b.HasOne(pt => pt.Employee)
                .WithMany(e => e.ProductionTask)
                .HasForeignKey(pt => pt.EmployeeId);

            b.HasOne(pt => pt.ProductionStage)
                .WithMany(ps => ps.ProductionTask)
                .HasForeignKey(pt => pt.ProductionStageId);
        });
    }
}
