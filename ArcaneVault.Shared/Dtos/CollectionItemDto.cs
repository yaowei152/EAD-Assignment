/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos;

/// <summary>Read model for a collection item, returned by GET endpoints (list/details).</summary>
public class CollectionItemDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int StartingQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<CategoryDto> Categories { get; set; } = new();
}
