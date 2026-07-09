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

public class IndexModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public IndexModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    public List<CollectionItemDto> Items { get; set; } = new();

    /// <summary>Bound from the query string so the search box works as a normal GET form (bookmarkable, no JS required).</summary>
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var userName = User.Identity!.Name!;
        var result = await _api.GetCollectionItemsAsync(userName, Search);

        if (result.Success && result.Data is not null)
        {
            Items = result.Data;
        }
        else
        {
            ErrorMessage ??= result.ErrorTitle;
        }
    }
}
