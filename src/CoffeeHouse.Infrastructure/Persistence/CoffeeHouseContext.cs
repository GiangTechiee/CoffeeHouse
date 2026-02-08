using CoffeeHouse.Domain.Entities;
using CoffeeHouse.Domain.Identity;
using CoffeeHouse.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace CoffeeHouse.Infrastructure.Persistence
{
    /// <summary>
    /// Refactored Database context for Coffee House Management System.
    /// Inherits from IdentityDbContext for security integration.
    /// </summary>
    public class CoffeeHouseContext : IdentityDbContext<AppUser, AppRole, int>
    {
        private readonly IMediator _mediator;

        public CoffeeHouseContext(DbContextOptions<CoffeeHouseContext> options, IMediator mediator) : base(options) 
        {
            _mediator = mediator;
        }

        // DbSet definitions
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<CafeStore> CafeStores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        // Legacy entities (to be migrated)
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> OldRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Identity table names
            modelBuilder.Entity<AppUser>(entity => { entity.ToTable("Users"); });
            modelBuilder.Entity<AppRole>(entity => { entity.ToTable("Roles"); });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<int>>(entity => { entity.ToTable("UserRoles"); });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<int>>(entity => { entity.ToTable("UserClaims"); });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<int>>(entity => { entity.ToTable("UserLogins"); });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>>(entity => { entity.ToTable("RoleClaims"); });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<int>>(entity => { entity.ToTable("UserTokens"); });

            // 1. Apply all configurations from the current assembly (Configurations/ folder)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 2. Configure DateTime to UTC conversion globally
            ConfigureDateTimeConversion(modelBuilder);

            // 3. Configure Global Query Filters for Soft Delete (BaseEntity)
            ConfigureGlobalFilters(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEvents();
            return result;
        }

        private async Task DispatchDomainEvents()
        {
            var domainEntities = ChangeTracker.Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            foreach (var entity in domainEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(CoffeeHouseContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);
                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity
        {
            modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted);
        }

        private void ConfigureDateTimeConversion(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}

