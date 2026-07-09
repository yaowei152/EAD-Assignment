/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data.Entities;
using ArcaneVault.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVault.Api.Data;

/// <summary>
/// Seeds the two fixed roles and one ready-to-use Staff account, plus a handful of sample
/// categories, so Category Management and the Analytics Dashboard are reachable immediately
/// after first run without a chicken-and-egg problem (self-registration only ever creates
/// User-role accounts).
/// </summary>
public static class SeedData
{
    public const string SeedStaffUserName = "admin";
    public const string SeedStaffPassword = "Admin@123";

    public static async Task InitializeAsync(ArcaneVaultDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.ArcaneVaultUserRoles.AnyAsync())
        {
            db.ArcaneVaultUserRoles.AddRange(
                new ArcaneVaultUserRole { RoleId = 1, RoleName = RoleNames.User },
                new ArcaneVaultUserRole { RoleId = 2, RoleName = RoleNames.Staff });
            await db.SaveChangesAsync();
        }

        if (!await db.ArcaneVaultUsers.AnyAsync())
        {
            var hasher = new PasswordHasher<ArcaneVaultUser>();
            var staffUser = new ArcaneVaultUser
            {
                UserName = SeedStaffUserName,
                Email = "admin@arcanevault.local",
                RoleId = 2, // Staff
                IsDeleted = false
            };
            staffUser.PasswordHash = hasher.HashPassword(staffUser, SeedStaffPassword);

            db.ArcaneVaultUsers.Add(staffUser);
            await db.SaveChangesAsync();
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { CategoryCode = "CARD", CategoryName = "Trading Cards" },
                new Category { CategoryCode = "COIN", CategoryName = "Coins & Currency" },
                new Category { CategoryCode = "FIG", CategoryName = "Action Figures" },
                new Category { CategoryCode = "COMIC", CategoryName = "Comics & Manga" },
                new Category { CategoryCode = "STAMP", CategoryName = "Stamps" });
            await db.SaveChangesAsync();
        }
    }
}
