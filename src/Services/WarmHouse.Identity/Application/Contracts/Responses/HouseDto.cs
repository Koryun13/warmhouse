namespace WarmHouse.Identity.Application.Contracts.Responses;

/// <summary>A home as the API publishes it.</summary>
public sealed record HouseDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    string? Address,
    string TimeZone,
    DateTimeOffset CreatedAt);
