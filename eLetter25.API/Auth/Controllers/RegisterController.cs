using eLetter25.API.Auth.Models;
using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Auth.Controllers;

/// <summary>
/// Controller for user registration
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class RegisterController(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext)
    : ControllerBase
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success confirmation</returns>
    /// <response code="200">User successfully registered</response>
    /// <response code="400">Validation error or registration failed</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EnableNotifications = request.EnableNotifications
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            return BadRequest(identityResult.Errors);
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, "User");
        if (!addToRoleResult.Succeeded)
        {
            return BadRequest(addToRoleResult.Errors);
        }

        await transaction.CommitAsync(cancellationToken);

        return Ok(new { message = "User successfully registered" });
    }
}