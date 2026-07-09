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

public class DetailsModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public DetailsModel(ArcaneVaultApiClient api)
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
}
