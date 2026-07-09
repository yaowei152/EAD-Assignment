/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos.Analytics;

/// <summary>One row of the "most popular items" analytics chart.</summary>
public class PopularItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public int TotalCurrentQuantity { get; set; }
    public int OwnerCount { get; set; }
}
