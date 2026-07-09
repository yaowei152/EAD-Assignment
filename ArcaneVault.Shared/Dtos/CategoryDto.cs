/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;

namespace ArcaneVault.Shared.Dtos;

/// <summary>Represents a collection category, used for both API responses and Create/Edit form binding.</summary>
public class CategoryDto
{
    [Required(ErrorMessage = "Category code is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Category code must be between 2 and 20 characters.")]
    public string CategoryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Number of (non-deleted) collection items currently tagged with this category. Read-only, set by the API.</summary>
    public int ItemCount { get; set; }
}
