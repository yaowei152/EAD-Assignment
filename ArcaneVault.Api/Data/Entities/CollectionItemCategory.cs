/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVault.Api.Data.Entities;

/// <summary>
/// Join entity between CollectionItem and Category (many-to-many). ItemId + CategoryCode
/// together form the composite primary key, configured explicitly in ArcaneVaultDbContext
/// since EF Core cannot infer a composite key by convention.
/// </summary>
[Table("CollectionItemCategories")]
public class CollectionItemCategory
{
    public int ItemId { get; set; }

    [MaxLength(20)]
    public string CategoryCode { get; set; } = string.Empty;

    public CollectionItem Item { get; set; } = null!;

    public Category Category { get; set; } = null!;
}
