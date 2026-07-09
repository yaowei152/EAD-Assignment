/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data;
using ArcaneVault.Api.Data.Entities;
using ArcaneVault.Shared;
using ArcaneVault.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVault.Api.Controllers;

/// <summary>
/// Registration and login endpoints. Stateless - issues no cookie or token itself.
/// The Web project (the only client of this API) owns the cookie session and builds it
/// from the UserDto returned here after a successful call.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ArcaneVaultDbContext _db;
    private readonly IPasswordHasher<ArcaneVaultUser> _passwordHasher;

    public AuthController(ArcaneVaultDbContext db, IPasswordHasher<ArcaneVaultUser> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _db.ArcaneVaultUsers.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
        {
            ModelState.AddModelError(nameof(request.Email), "This email address is already registered.");
            return Conflict(new ValidationProblemDetails(ModelState) { Title = "Registration failed." });
        }

        if (await _db.ArcaneVaultUsers.AnyAsync(u => u.UserName == request.UserName))
        {
            ModelState.AddModelError(nameof(request.UserName), "This username is already taken.");
            return Conflict(new ValidationProblemDetails(ModelState) { Title = "Registration failed." });
        }

        var user = new ArcaneVaultUser
        {
            UserName = request.UserName.Trim(),
            Email = request.Email.Trim(),
            RoleId = 1, // self-registration always creates a plain "User" account
            IsDeleted = false
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.ArcaneVaultUsers.Add(user);
        await _db.SaveChangesAsync();

        var role = await _db.ArcaneVaultUserRoles.FindAsync(user.RoleId);

        return CreatedAtAction(nameof(Register), new UserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            RoleName = role?.RoleName ?? RoleNames.User
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _db.ArcaneVaultUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.UserName && !u.IsDeleted);

        if (user is null)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid username or password." });
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid username or password." });
        }

        return Ok(new UserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            RoleName = user.Role.RoleName
        });
    }
}
