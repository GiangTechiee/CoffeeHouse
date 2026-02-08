using CoffeeHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeHouse.Infrastructure.Persistence.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(h => h.OrderId);

        builder.Property(h => h.OrderDate)
            .HasColumnType("TIMESTAMPTZ");

        builder.Property(h => h.TotalAmount)
            .HasColumnType("NUMERIC(12,2)");

        builder.Property(h => h.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(h => h.PaymentMethod)
            .HasMaxLength(50);

        // Audit
        builder.Property(n => n.CreatedAt)
            .HasColumnType("TIMESTAMPTZ");

        builder.Property(n => n.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ");

        // Relationships
        builder.HasMany(h => h.SalesOrderItems)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Store)
            .WithMany(q => q.SalesOrders)
            .HasForeignKey(h => h.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Employee)
            .WithMany(n => n.SalesOrders)
            .HasForeignKey(h => h.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(h => h.Customer)
            .WithMany(k => k.SalesOrders)
            .HasForeignKey(h => h.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(h => h.StoreId).HasDatabaseName("IX_SalesOrder_StoreId");
        builder.HasIndex(h => h.EmployeeId).HasDatabaseName("IX_SalesOrder_EmployeeId");
        builder.HasIndex(h => h.CustomerId).HasDatabaseName("IX_SalesOrder_CustomerId");
        builder.HasIndex(h => h.Status).HasDatabaseName("IX_SalesOrder_Status");
        builder.HasIndex(h => h.OrderDate).HasDatabaseName("IX_SalesOrder_OrderDate");

        builder.HasIndex(o => new { o.CustomerId, o.OrderDate })
            .HasDatabaseName("IX_SalesOrders_CustomerId_OrderDate");

        builder.HasIndex(o => new { o.StoreId, o.OrderDate })
            .HasDatabaseName("IX_SalesOrders_StoreId_OrderDate");
    }
}
