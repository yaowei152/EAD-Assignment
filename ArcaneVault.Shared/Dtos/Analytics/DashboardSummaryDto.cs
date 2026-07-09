/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos.Analytics;

/// <summary>Top-level KPI tiles shown at the top of the Staff analytics dashboard.</summary>
public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }
    public int TotalCategories { get; set; }
    public int TotalItems { get; set; }
    public int TotalQuantityAcrossAllItems { get; set; }
}
