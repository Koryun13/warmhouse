using WarmHouse.Identity.Domain.Houses;

namespace WarmHouse.Identity.Application.Contracts;

public sealed record RegisterUserRequest(string Email, string DisplayName, string Password);

public sealed record UserDto(Guid Id, string Email, string DisplayName, DateTimeOffset CreatedAt);

public sealed record TokenRequest(string Email, string Password);

public sealed record TokenDto(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);

/// <summary>The owner is the authenticated caller, never a value from the body.</summary>
public sealed record CreateHouseRequest(string Name, string? Address, string? TimeZone);

public sealed record HouseDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    string? Address,
    string TimeZone,
    DateTimeOffset CreatedAt);

public sealed record GrantAccessRequest(Guid UserId, HouseRole Role);
