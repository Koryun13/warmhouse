namespace WarmHouse.Identity.Application.Contracts.Responses;

/// <summary>The issued access token and when it stops being valid.</summary>
public sealed record TokenDto(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
