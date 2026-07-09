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
/// Master-data CRUD for collection categories. Every action here is reachable only from
/// Staff-restricted Razor Pages on the Web side; the API itself trusts that gate (see README
/// for the documented Web-owns-identity simplification) but still protects referential
/// integrity itself - a category in use can never be deleted regardless of who calls this.
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ArcaneVaultDbContext _db;

    public CategoriesController(ArcaneVaultDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.CategoryName)
            .Select(c => new CategoryDto
            {
                CategoryCode = c.CategoryCode,
                CategoryName = c.CategoryName,
                ItemCount = c.ItemCategories.Count(ic => !ic.Item.IsDeleted)
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<CategoryDto>> GetByCode(string code)
    {
        var category = await _db.Categories
            .Where(c => c.CategoryCode == code)
            .Select(c => new CategoryDto
            {
                CategoryCode = c.CategoryCode,
                CategoryName = c.CategoryName,
                ItemCount = c.ItemCategories.Count(ic => !ic.Item.IsDeleted)
            })
            .FirstOrDefaultAsync();

        if (category is null)
        {
            return NotFound(new ProblemDetails { Title = $"Category '{code}' was not found." });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (await _db.Categories.AnyAsync(c => c.CategoryCode == dto.CategoryCode))
        {
            ModelState.AddModelError(nameof(dto.CategoryCode), "A category with this code already exists.");
            return Conflict(new ValidationProblemDetails(ModelState) { Title = "Category creation failed." });
        }

        var category = new Category
        {
            CategoryCode = dto.CategoryCode.Trim(),
            CategoryName = dto.CategoryName.Trim()
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCode), new { code = category.CategoryCode },
            new CategoryDto { CategoryCode = category.CategoryCode, CategoryName = category.CategoryName, ItemCount = 0 });
    }

    [HttpPut("{code}")]
    public async Task<ActionResult<CategoryDto>> Update(string code, [FromBody] CategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.CategoryCode == code);
        if (category is null)
        {
            return NotFound(new ProblemDetails { Title = $"Category '{code}' was not found." });
        }

        category.CategoryName = dto.CategoryName.Trim();
        await _db.SaveChangesAsync();

        var itemCount = await _db.CollectionItemCategories.CountAsync(cic => cic.CategoryCode == code && !cic.Item.IsDeleted);
        return Ok(new CategoryDto { CategoryCode = category.CategoryCode, CategoryName = category.CategoryName, ItemCount = itemCount });
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.CategoryCode == code);
        if (category is null)
        {
            return NotFound(new ProblemDetails { Title = $"Category '{code}' was not found." });
        }

        var isInUse = await _db.CollectionItemCategories.AnyAsync(cic => cic.CategoryCode == code && !cic.Item.IsDeleted);
        if (isInUse)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Cannot delete category.",
                Detail = $"Category '{code}' is still assigned to one or more collection items. Remove it from those items first."
            });
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
