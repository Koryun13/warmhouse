using WarmHouse.Identity.Application.Contracts.Responses;

namespace WarmHouse.Identity.Application.Mapping;

/// <summary>Projects an issued token onto its published shape.</summary>
public static class TokenMapper
{
    private const string BearerScheme = "Bearer";

    public static TokenDto ToDto(string accessToken, DateTimeOffset expiresAt) => new(
        accessToken,
        BearerScheme,
        expiresAt);
}
