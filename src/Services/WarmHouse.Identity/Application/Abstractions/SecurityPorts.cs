using WarmHouse.Identity.Domain.Users;

namespace WarmHouse.Identity.Application.Abstractions;

/// <summary>Password hashing, kept behind a port so the algorithm can be replaced.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}

/// <summary>Issues the access token that other services trust.</summary>
public interface ITokenIssuer
{
    (string Token, DateTimeOffset ExpiresAt) Issue(User user, IReadOnlyCollection<Guid> houseIds);
}
