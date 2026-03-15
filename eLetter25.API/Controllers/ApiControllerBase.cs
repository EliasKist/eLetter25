using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace eLetter25.API.Controllers;

/// <summary>
/// Base class for all API controllers. Provides shared helpers for authenticated endpoints.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Attempts to parse the authenticated user's ID from the JWT claims as a <see cref="Guid"/>.
    /// Returns <c>false</c> when the claim is absent or not a valid GUID, which should be
    /// treated as a 401 response.
    /// </summary>
    protected bool TryGetOwnerId(out Guid ownerId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out ownerId);
    }
}

