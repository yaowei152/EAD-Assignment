/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVault.Api.Data;

public class ArcaneVaultDbContext : DbContext
{
    public ArcaneVaultDbContext(DbContextOptions<ArcaneVaultDbContext> options) : base(options)
    {
    }

    public DbSet<ArcaneVaultUser> ArcaneVaultUsers => Set<ArcaneVaultUser>();
    public DbSet<ArcaneVaultUserRole> ArcaneVaultUserRoles => Set<ArcaneVaultUserRole>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
    public DbSet<CollectionItemCategory> CollectionItemCategories => Set<CollectionItemCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CollectionItemCategories: composite primary key (ItemId, CategoryCode).
        modelBuilder.Entity<CollectionItemCategory>()
            .HasKey(cic => new { cic.ItemId, cic.CategoryCode });

        modelBuilder.Entity<CollectionItemCategory>()
            .HasOne(cic => cic.Item)
            .WithMany(i => i.ItemCategories)
            .HasForeignKey(cic => cic.ItemId)
            .OnDelete(DeleteBehavior.Cascade); // deleting an item cleans up its category links

        modelBuilder.Entity<CollectionItemCategory>()
            .HasOne(cic => cic.Category)
            .WithMany(c => c.ItemCategories)
            .HasForeignKey(cic => cic.CategoryCode)
            .OnDelete(DeleteBehavior.Restrict); // a category in use cannot be deleted (checked explicitly in the controller)

        modelBuilder.Entity<ArcaneVaultUser>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CollectionItem>()
            .HasOne(i => i.User)
            .WithMany(u => u.CollectionItems)
            .HasForeignKey(i => i.UserName)
            .OnDelete(DeleteBehavior.Restrict); // users are soft-deleted, never hard-deleted, so no cascade needed

        modelBuilder.Entity<ArcaneVaultUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
