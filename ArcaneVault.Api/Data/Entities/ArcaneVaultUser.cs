/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVault.Api.Data.Entities;

/// <summary>A registered user (shop owner or collector, or Staff/administrator).</summary>
[Table("ArcaneVaultUsers")]
public class ArcaneVaultUser
{
    [Key]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Salted hash produced by <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/>.
    /// Not part of the assignment's minimum column list, but required to authenticate anyone.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    public ArcaneVaultUserRole Role { get; set; } = null!;

    public ICollection<CollectionItem> CollectionItems { get; set; } = new List<CollectionItem>();
}
