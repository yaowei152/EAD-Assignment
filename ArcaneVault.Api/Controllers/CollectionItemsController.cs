/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data;
using ArcaneVault.Api.Data.Entities;
using ArcaneVault.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVault.Api.Controllers;

/// <summary>
/// CRUD for a user's personal collection items. Every action requires a userName (the Web
/// layer supplies the signed-in principal's name on every call - see README for the documented
/// identity-trust simplification) and every read/update/delete is scoped so a user can only
/// ever touch their own rows, regardless of what id is requested.
/// </summary>
[ApiController]
[Route("api/collectionitems")]
public class CollectionItemsController : ControllerBase
{
    private readonly ArcaneVaultDbContext _db;

    public CollectionItemsController(ArcaneVaultDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<CollectionItemDto>>> GetAll([FromQuery] string userName, [FromQuery] string? search)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return BadRequest(new ProblemDetails { Title = "userName is required." });
        }

        var query = _db.CollectionItems
            .Include(i => i.ItemCategories).ThenInclude(ic => ic.Category)
            .Where(i => i.UserName == userName && !i.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(i =>
                EF.Functions.Like(i.ItemName, $"%{term}%") ||
                i.ItemCategories.Any(ic => EF.Functions.Like(ic.Category.CategoryName, $"%{term}%")) ||
                EF.Functions.Like(i.StartingQuantity.ToString(), $"%{term}%") ||
                EF.Functions.Like(i.CurrentQuantity.ToString(), $"%{term}%"));
        }

        var items = await query.OrderBy(i => i.ItemName).ToListAsync();
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CollectionItemDto>> GetById(int id, [FromQuery] string userName)
    {
        var item = await _db.CollectionItems
            .Include(i => i.ItemCategories).ThenInclude(ic => ic.Category)
            .FirstOrDefaultAsync(i => i.ItemId == id && !i.IsDeleted);

        // Return 404 (not 403) for items that exist but belong to someone else, so an id alone
        // never reveals whether it exists in another user's collection.
        if (item is null || item.UserName != userName)
        {
            return NotFound(new ProblemDetails { Title = $"Collection item {id} was not found." });
        }

        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<CollectionItemDto>> Create([FromBody] CollectionItemUpsertDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var ownerExists = await _db.ArcaneVaultUsers.AnyAsync(u => u.UserName == dto.UserName && !u.IsDeleted);
        if (!ownerExists)
        {
            return BadRequest(new ProblemDetails { Title = "Owning user does not exist." });
        }

        var item = new CollectionItem
        {
            ItemName = dto.ItemName.Trim(),
            StartingQuantity = dto.StartingQuantity,
            CurrentQuantity = dto.CurrentQuantity,
            UserName = dto.UserName,
            IsDeleted = false
        };

        await SyncCategoriesAsync(item, dto.CategoryCodes);

        _db.CollectionItems.Add(item);
        await _db.SaveChangesAsync();

        var created = await _db.CollectionItems
            .Include(i => i.ItemCategories).ThenInclude(ic => ic.Category)
            .FirstAsync(i => i.ItemId == item.ItemId);

        return CreatedAtAction(nameof(GetById), new { id = item.ItemId, userName = item.UserName }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CollectionItemDto>> Update(int id, [FromBody] CollectionItemUpsertDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var item = await _db.CollectionItems
            .Include(i => i.ItemCategories)
            .FirstOrDefaultAsync(i => i.ItemId == id && !i.IsDeleted);

        if (item is null || item.UserName != dto.UserName)
        {
            return NotFound(new ProblemDetails { Title = $"Collection item {id} was not found." });
        }

        item.ItemName = dto.ItemName.Trim();
        item.CurrentQuantity = dto.CurrentQuantity;
        // StartingQuantity is an immutable historical baseline set at creation - intentionally not updated here.

        await SyncCategoriesAsync(item, dto.CategoryCodes);

        await _db.SaveChangesAsync();

        var updated = await _db.CollectionItems
            .Include(i => i.ItemCategories).ThenInclude(ic => ic.Category)
            .FirstAsync(i => i.ItemId == id);

        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string userName)
    {
        var item = await _db.CollectionItems.FirstOrDefaultAsync(i => i.ItemId == id && !i.IsDeleted);

        if (item is null || item.UserName != userName)
        {
            return NotFound(new ProblemDetails { Title = $"Collection item {id} was not found." });
        }

        // Soft delete: the IsDeleted flag preserves history instead of removing the row.
        item.IsDeleted = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private async Task SyncCategoriesAsync(CollectionItem item, List<string> categoryCodes)
    {
        var distinctCodes = categoryCodes.Distinct().ToList();

        if (distinctCodes.Count > 0)
        {
            var validCodes = await _db.Categories
                .Where(c => distinctCodes.Contains(c.CategoryCode))
                .Select(c => c.CategoryCode)
                .ToListAsync();
            distinctCodes = validCodes;
        }

        item.ItemCategories.Clear();
        foreach (var code in distinctCodes)
        {
            item.ItemCategories.Add(new CollectionItemCategory { CategoryCode = code });
        }
    }

    private static CollectionItemDto ToDto(CollectionItem item) => new()
    {
        ItemId = item.ItemId,
        ItemName = item.ItemName,
        StartingQuantity = item.StartingQuantity,
        CurrentQuantity = item.CurrentQuantity,
        UserName = item.UserName,
        Categories = item.ItemCategories
            .Select(ic => new CategoryDto { CategoryCode = ic.CategoryCode, CategoryName = ic.Category.CategoryName })
            .OrderBy(c => c.CategoryName)
            .ToList()
    };
}
