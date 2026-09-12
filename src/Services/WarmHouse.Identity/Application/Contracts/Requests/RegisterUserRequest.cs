namespace WarmHouse.Identity.Application.Contracts.Requests;

/// <summary>Self-service sign-up.</summary>
public sealed record RegisterUserRequest(string Email, string DisplayName, string Password);
