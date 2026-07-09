/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos.Analytics;

/// <summary>One row of the "category demand" analytics chart.</summary>
public class CategoryDemandDto
{
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
}
