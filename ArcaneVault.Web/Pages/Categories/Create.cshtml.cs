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

public class CreateModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public CreateModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public CategoryDto Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _api.CreateCategoryAsync(Input);

        if (!result.Success)
        {
            ApplyApiErrors(result);
            return Page();
        }

        TempData["SuccessMessage"] = $"Category '{Input.CategoryCode}' was created.";
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
            ModelState.AddModelError(string.Empty, result.ErrorTitle ?? "Could not create category.");
        }
    }
}
