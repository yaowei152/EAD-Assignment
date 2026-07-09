/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Shared.Dtos;
using ArcaneVault.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.CollectionItems;

public class EditModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public EditModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public CollectionItemUpsertDto Input { get; set; } = new();

    public List<CategoryDto> AvailableCategories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userName = User.Identity!.Name!;
        var result = await _api.GetCollectionItemAsync(id, userName);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "That item was not found in your collection.";
            return RedirectToPage("Index");
        }

        Input = new CollectionItemUpsertDto
        {
            ItemName = result.Data.ItemName,
            StartingQuantity = result.Data.StartingQuantity,
            CurrentQuantity = result.Data.CurrentQuantity,
            UserName = userName,
            CategoryCodes = result.Data.Categories.Select(c => c.CategoryCode).ToList()
        };

        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userName = User.Identity!.Name!;
        ModelState.Remove($"{nameof(Input)}.{nameof(Input.UserName)}");
        Input.UserName = userName;

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        var result = await _api.UpdateCollectionItemAsync(id, Input);

        if (!result.Success)
        {
            ApplyApiErrors(result);
            await LoadCategoriesAsync();
            return Page();
        }

        TempData["SuccessMessage"] = $"'{Input.ItemName}' was updated.";
        return RedirectToPage("Index");
    }

    private async Task LoadCategoriesAsync()
    {
        var result = await _api.GetCategoriesAsync();
        AvailableCategories = result.Data ?? new List<CategoryDto>();
    }

    private void ApplyApiErrors(ApiResult<CollectionItemDto> result)
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
            ModelState.AddModelError(string.Empty, result.ErrorTitle ?? "Could not update this item.");
        }
    }
}
