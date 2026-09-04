using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Application.Abstractions;

/// <summary>Issues the access token that other services trust.</summary>
public interface ITokenIssuer
{
    (string Token, DateTimeOffset ExpiresAt) Issue(User user, IReadOnlyCollection<Guid> houseIds);
}
