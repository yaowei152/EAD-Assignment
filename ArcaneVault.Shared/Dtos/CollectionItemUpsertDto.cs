/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;

namespace ArcaneVault.Shared.Dtos;

/// <summary>
/// Write model for POST/PUT collection item requests. UserName is the owning user and is
/// supplied by the Web layer from the signed-in principal (not user-editable in the form) -
/// the API re-checks it against the existing row's owner on update/delete.
/// </summary>
public class CollectionItemUpsertDto
{
    [Required(ErrorMessage = "Item name is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Item name must be at most 100 characters.")]
    public string ItemName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Starting quantity is required.")]
    [Range(0, 1_000_000, ErrorMessage = "Starting quantity must be a non-negative number.")]
    public int StartingQuantity { get; set; }

    [Required(ErrorMessage = "Current quantity is required.")]
    [Range(0, 1_000_000, ErrorMessage = "Current quantity must be a non-negative number.")]
    public int CurrentQuantity { get; set; }

    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>Category codes selected for this item. May be empty.</summary>
    public List<string> CategoryCodes { get; set; } = new();
}
