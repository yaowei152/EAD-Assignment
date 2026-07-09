/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data;
using ArcaneVault.Shared.Dtos.Analytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVault.Api.Controllers;

/// <summary>
/// Platform-wide aggregation endpoints backing the Staff analytics dashboard - the
/// Propose-a-Feature component. Every query aggregates across ALL users (not scoped to a
/// single owner), which is exactly why these actions are Staff-only on the Web side: they
/// expose cross-user insight that individual Users should not see.
/// </summary>
[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly ArcaneVaultDbContext _db;

    public AnalyticsController(ArcaneVaultDbContext db)
    {
        _db = db;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var summary = new DashboardSummaryDto
        {
            TotalUsers = await _db.ArcaneVaultUsers.CountAsync(u => !u.IsDeleted),
            TotalCategories = await _db.Categories.CountAsync(),
            TotalItems = await _db.CollectionItems.CountAsync(i => !i.IsDeleted),
            TotalQuantityAcrossAllItems = await _db.CollectionItems
                .Where(i => !i.IsDeleted)
                .SumAsync(i => (int?)i.CurrentQuantity) ?? 0
        };

        return Ok(summary);
    }

    [HttpGet("popular-items")]
    public async Task<ActionResult<List<PopularItemDto>>> GetPopularItems([FromQuery] int top = 10)
    {
        var items = await _db.CollectionItems
            .Where(i => !i.IsDeleted)
            .GroupBy(i => i.ItemName)
            .Select(g => new PopularItemDto
            {
                ItemName = g.Key,
                TotalCurrentQuantity = g.Sum(i => i.CurrentQuantity),
                OwnerCount = g.Select(i => i.UserName).Distinct().Count()
            })
            .OrderByDescending(x => x.TotalCurrentQuantity)
            .ThenBy(x => x.ItemName)
            .Take(top)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("category-demand")]
    public async Task<ActionResult<List<CategoryDemandDto>>> GetCategoryDemand()
    {
        var demand = await _db.Categories
            .Select(c => new CategoryDemandDto
            {
                CategoryCode = c.CategoryCode,
                CategoryName = c.CategoryName,
                ItemCount = c.ItemCategories.Count(ic => !ic.Item.IsDeleted),
                TotalQuantity = c.ItemCategories
                    .Where(ic => !ic.Item.IsDeleted)
                    .Sum(ic => (int?)ic.Item.CurrentQuantity) ?? 0
            })
            .OrderByDescending(c => c.TotalQuantity)
            .ThenBy(c => c.CategoryName)
            .ToListAsync();

        return Ok(demand);
    }

    [HttpGet("user-activity")]
    public async Task<ActionResult<List<UserActivityDto>>> GetUserActivity([FromQuery] int top = 10)
    {
        var activity = await _db.CollectionItems
            .Where(i => !i.IsDeleted)
            .GroupBy(i => i.UserName)
            .Select(g => new UserActivityDto
            {
                UserName = g.Key,
                ItemCount = g.Count(),
                TotalQuantityHeld = g.Sum(i => i.CurrentQuantity)
            })
            .OrderByDescending(x => x.TotalQuantityHeld)
            .ThenBy(x => x.UserName)
            .Take(top)
            .ToListAsync();

        return Ok(activity);
    }
}
