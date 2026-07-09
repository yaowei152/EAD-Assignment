/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos.Analytics;

/// <summary>One row of the "most active collectors" analytics chart.</summary>
public class UserActivityDto
{
    public string UserName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantityHeld { get; set; }
}
