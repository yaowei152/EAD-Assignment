/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using System.ComponentModel.DataAnnotations;

namespace ArcaneVault.Shared.Dtos.Auth;

/// <summary>Request body for POST /api/auth/login.</summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
