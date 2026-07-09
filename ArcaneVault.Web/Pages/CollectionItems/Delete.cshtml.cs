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

public class DeleteModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public DeleteModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    public CollectionItemDto? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userName = User.Identity!.Name!;
        var result = await _api.GetCollectionItemAsync(id, userName);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "That item was not found in your collection.";
            return RedirectToPage("Index");
        }

        Item = result.Data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userName = User.Identity!.Name!;
        var result = await _api.DeleteCollectionItemAsync(id, userName);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorTitle ?? "Unable to delete this item.";
            return RedirectToPage("Index");
        }

        TempData["SuccessMessage"] = "Item was removed from your collection.";
        return RedirectToPage("Index");
    }
}
