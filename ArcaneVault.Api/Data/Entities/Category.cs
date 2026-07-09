/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVault.Api.Data.Entities;

/// <summary>A collection category (master data maintained by Staff), e.g. "Trading Cards", "Coins".</summary>
[Table("Categories")]
public class Category
{
    [Key]
    [MaxLength(20)]
    public string CategoryCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public ICollection<CollectionItemCategory> ItemCategories { get; set; } = new List<CollectionItemCategory>();
}
