/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Shared.Dtos.Auth;
using ArcaneVault.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public RegisterModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public RegisterRequestDto Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _api.RegisterAsync(Input);

        if (!result.Success)
        {
            ApplyApiErrors(result);
            return Page();
        }

        TempData["SuccessMessage"] = "Registration successful! You may now log in.";
        return RedirectToPage("/Account/Login");
    }

    private void ApplyApiErrors(ApiResult<UserDto> result)
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
            ModelState.AddModelError(string.Empty, result.ErrorTitle ?? "Registration failed. Please try again.");
        }
    }
}
