/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared.Dtos.Auth;

/// <summary>
/// Returned by the API after a successful register/login. The Web project uses this
/// to build the signed-in user's claims (name + role) for its own cookie session.
/// </summary>
public class UserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
