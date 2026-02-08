using CoffeeHouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeHouse.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.ProductId);
        builder.Property(p => p.ProductId).UseIdentityColumn();

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Price)
            .HasColumnType("NUMERIC(12,2)");

        builder.Property(p => p.Description)
            .HasColumnType("TEXT");

        builder.Property(n => n.CreatedAt)
            .HasColumnType("TIMESTAMPTZ");

        builder.Property(n => n.UpdatedAt)
            .HasColumnType("TIMESTAMPTZ");

        builder.Property(p => p.Notes)
            .HasColumnType("TEXT");

        builder.Property(p => p.Status)
            .HasMaxLength(20)
            .HasDefaultValue("Active");

        // Relationships
        builder.HasOne(s => s.Category)
            .WithMany(n => n.Products)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.CategoryId).HasDatabaseName("IX_Product_CategoryId");
        builder.HasIndex(s => s.ProductName).HasDatabaseName("IX_Product_ProductName");
        builder.HasIndex(s => s.Status).HasDatabaseName("IX_Product_Status");
        builder.HasIndex(s => s.Price).HasDatabaseName("IX_Product_Price");
        
        builder.HasIndex(p => new { p.CategoryId, p.Status })
            .HasDatabaseName("IX_Products_CategoryId_Status");

        // Full-text search index (PostgreSQL specific)
        builder.HasIndex(p => p.ProductName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Products_ProductName_FullText");

        builder.HasIndex(p => p.Description)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Products_Description_FullText");
    }
}
