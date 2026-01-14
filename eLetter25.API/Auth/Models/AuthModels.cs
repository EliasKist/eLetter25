namespace eLetter25.API.Auth.Models;

/// <summary>
/// Request model for user registration
/// </summary>
/// <param name="Email">User's email address</param>
/// <param name="Password">Password</param>
/// <param name="EnableNotifications">Enable notifications</param>
public sealed record RegisterRequest(
    string Email,
    string Password,
    bool EnableNotifications = false);

/// <summary>
/// Request model for login
/// </summary>
/// <param name="Email">Email address</param>
/// <param name="Password">Password</param>
public sealed record LoginRequest(string Email, string Password);

/// <summary>
/// Response model for login
/// </summary>
/// <param name="AccessToken">JWT Access Token</param>
public sealed record LoginResponse(string AccessToken);