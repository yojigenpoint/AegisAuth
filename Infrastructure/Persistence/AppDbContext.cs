using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using YojigenPoint.Aegisauth.Domain.Common;
using YojigenPoint.Aegisauth.Domain.Entities;

namespace YojigenPoint.AegisAuth.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ----- User Configuration -----
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasConversion(g => g.ToString(), t => Guid.Parse(t));
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();

            builder.HasMany(u => u.Roles)
                   .WithMany(r => r.Users)
                   .UsingEntity(j => j.ToTable("UserRoles"));
        });

        // ----- Role Configuration -----
        modelBuilder.Entity<Role>(builder =>
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.Id);
            // NOTE: For simplicity, Role ID is an auto-incrementing integer.
            // If you wanted GUIDs for Roles too, you would change the Id type
            // in the Role entity and configure it here like the User Id.
            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.HasIndex(r => r.Name).IsUnique();
            builder.Property(r => r.Name).IsRequired();
        });

        // ----- RefreshToken Configuration -----
        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).HasConversion(g => g.ToString(), t => Guid.Parse(t));
            builder.HasIndex(rt => rt.Token).IsUnique();
            builder.Property(rt => rt.Token).IsRequired();

            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(rt => rt.UserId);
        });

        // Configure soft delete for all entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Create the lambda parameter: e.g., "p"
                var parameter = Expression.Parameter(entityType.ClrType, "p");

                // Create the property access: e.g., "p.DeletedAtUtc"
                var property = Expression.Property(parameter, nameof(BaseEntity.DeletedAtUtc));

                // Create the constant value: e.g., "null"
                var nullValue = Expression.Constant(null, typeof(DateTime?));

                // Create the comparison: e.g., "p.DeletedAtUtc == null"
                var equality = Expression.Equal(property, nullValue);

                // Build the complete lambda expression: e.g., "p => p.DeletedAtUtc == null"
                var lambda = Expression.Lambda(equality, parameter);

                // Apply the filter
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    // This override automatically sets the audit fields before saving changes to the database.
    // The DbContext acts as the Unit of Work, and this method commits all tracked changes.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = utcNow;
                    entry.Entity.ModifiedAtUtc = utcNow; // Set ModifiedAt on creation
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = utcNow;
                    break;
                case EntityState.Deleted:
                    // Intercept the delete command
                    entry.State = EntityState.Modified; // Change state to Modified
                    entry.Entity.DeletedAtUtc = utcNow; // Set the soft delete flag
                    break;
            }
        }
    }
}
