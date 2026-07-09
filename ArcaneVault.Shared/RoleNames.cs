/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

namespace ArcaneVault.Shared;

/// <summary>
/// Well-known role name constants shared by the Api (seeding, role checks) and the
/// Web project ([Authorize(Roles = ...)], NavBar visibility) so the strings only live in one place.
/// </summary>
public static class RoleNames
{
    public const string User = "User";
    public const string Staff = "Staff";
}
