/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Shared.Dtos.Analytics;
using ArcaneVault.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Analytics;

/// <summary>
/// Staff-only analytics dashboard - the Propose-a-Feature component. Aggregates platform-wide
/// insight (popular items, category demand, collector activity) directly from the business
/// goals stated in the assignment background. All four calls happen server-side through
/// ArcaneVaultApiClient, same as every other page, so the browser never talks to the API directly.
/// </summary>
public class DashboardModel : PageModel
{
    private readonly ArcaneVaultApiClient _api;

    public DashboardModel(ArcaneVaultApiClient api)
    {
        _api = api;
    }

    public DashboardSummaryDto Summary { get; set; } = new();
    public List<PopularItemDto> PopularItems { get; set; } = new();
    public List<CategoryDemandDto> CategoryDemand { get; set; } = new();
    public List<UserActivityDto> UserActivity { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var summaryResult = await _api.GetDashboardSummaryAsync();
        var popularResult = await _api.GetPopularItemsAsync(10);
        var demandResult = await _api.GetCategoryDemandAsync();
        var activityResult = await _api.GetUserActivityAsync(10);

        if (summaryResult.Success && summaryResult.Data is not null)
        {
            Summary = summaryResult.Data;
        }

        if (popularResult.Success && popularResult.Data is not null)
        {
            PopularItems = popularResult.Data;
        }

        if (demandResult.Success && demandResult.Data is not null)
        {
            CategoryDemand = demandResult.Data;
        }

        if (activityResult.Success && activityResult.Data is not null)
        {
            UserActivity = activityResult.Data;
        }

        if (!summaryResult.Success || !popularResult.Success || !demandResult.Success || !activityResult.Success)
        {
            ErrorMessage = "Some analytics data could not be loaded.";
        }
    }
}
