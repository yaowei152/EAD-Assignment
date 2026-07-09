/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVault.Api.Data.Entities;

/// <summary>Lookup table of user roles (User, Staff).</summary>
[Table("ArcaneVaultUserRoles")]
public class ArcaneVaultUserRole
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;

    public ICollection<ArcaneVaultUser> Users { get; set; } = new List<ArcaneVaultUser>();
}
