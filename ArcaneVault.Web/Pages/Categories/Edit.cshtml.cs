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

public class EditModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public EditModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public CategoryDto Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string code)
    {
        var result = await _api.GetCategoryAsync(code);
        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = $"Category '{code}' was not found.";
            return RedirectToPage("Index");
        }

        Input = result.Data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string code)
    {
        // CategoryCode is the primary key and is rendered read-only in the form; always
        // resolve the row to update from the route value, never from client-submitted input.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _api.UpdateCategoryAsync(code, Input);

        if (!result.Success)
        {
            ApplyApiErrors(result);
            return Page();
        }

        TempData["SuccessMessage"] = $"Category '{code}' was updated.";
        return RedirectToPage("Index");
    }

    private void ApplyApiErrors(ApiResult<CategoryDto> result)
    {
        if (result.ValidationErrors is { Count: > 0 })
        {
            foreach (var (field, messages) in result.ValidationErrors)
            {
                var modelKey = $"{nameof(Input)}.{field}";
                foreach (var message in messages)
                {
                    ModelState.AddModelError(modelKey, message);
                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.ErrorTitle ?? "Could not update category.");
        }
    }
}
