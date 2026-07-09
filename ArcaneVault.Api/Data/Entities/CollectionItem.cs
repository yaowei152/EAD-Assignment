/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVault.Api.Data.Entities;

/// <summary>A single collectible item owned by a user.</summary>
[Table("CollectionItems")]
public class CollectionItem
{
    [Key]
    public int ItemId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public int StartingQuantity { get; set; }

    public int CurrentQuantity { get; set; }

    [ForeignKey(nameof(User))]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    public ArcaneVaultUser User { get; set; } = null!;

    public ICollection<CollectionItemCategory> ItemCategories { get; set; } = new List<CollectionItemCategory>();
}
