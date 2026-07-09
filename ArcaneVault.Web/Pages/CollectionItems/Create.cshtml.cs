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

public class CreateModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public CreateModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public CollectionItemUpsertDto Input { get; set; } = new();

    public List<CategoryDto> AvailableCategories { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // UserName is never taken from the client: the owner of a new item is always the
        // signed-in principal, so any stale/absent binding for it is cleared and overwritten here.
        ModelState.Remove($"{nameof(Input)}.{nameof(Input.UserName)}");
        Input.UserName = User.Identity!.Name!;

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        var result = await _api.CreateCollectionItemAsync(Input);

        if (!result.Success)
        {
            ApplyApiErrors(result);
            await LoadCategoriesAsync();
            return Page();
        }

        TempData["SuccessMessage"] = $"'{Input.ItemName}' was added to your collection.";
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
            ModelState.AddModelError(string.Empty, result.ErrorTitle ?? "Could not add this item.");
        }
    }
}
