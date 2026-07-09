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

public class IndexModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public IndexModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    public List<CategoryDto> Categories { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _api.GetCategoriesAsync();
        if (result.Success && result.Data is not null)
        {
            Categories = result.Data;
        }
        else
        {
            ErrorMessage = result.ErrorTitle;
        }
    }
}
