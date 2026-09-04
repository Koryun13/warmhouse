namespace WarmHouse.Identity.Application.Contracts.Responses;

/// <summary>An account holder as the API publishes it; never the password hash.</summary>
public sealed record UserDto(Guid Id, string Email, string DisplayName, DateTimeOffset CreatedAt);
