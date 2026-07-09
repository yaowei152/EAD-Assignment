/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Shared.Dtos;
using ArcaneVault.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Categories;

public class DetailsModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public DetailsModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    public CategoryDto? Category { get; set; }

    public async Task<IActionResult> OnGetAsync(string code)
    {
        var result = await _api.GetCategoryAsync(code);
        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = $"Category '{code}' was not found.";
            return RedirectToPage("Index");
        }

        Category = result.Data;
        return Page();
    }
}
