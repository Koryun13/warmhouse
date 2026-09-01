using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Domain.Users;

namespace WarmHouse.Identity.Infrastructure.Security;

/// <summary>Issues signed JWTs carrying the houses a user may reach.</summary>
internal sealed class JwtTokenIssuer(IConfiguration configuration) : ITokenIssuer
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(8);

    private readonly string _issuer = configuration["JWT_ISSUER"] ?? "warmhouse-identity";
    private readonly string _audience = configuration["JWT_AUDIENCE"] ?? "warmhouse";

    // Development fallback only; a deployment must supply JWT_SIGNING_KEY.
    private readonly string _signingKey = configuration["JWT_SIGNING_KEY"]
        ?? "dev-only-signing-key-change-me-in-production-32b";

    public (string Token, DateTimeOffset ExpiresAt) Issue(User user, IReadOnlyCollection<Guid> houseIds)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(Lifetime);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
        };

        claims.AddRange(houseIds.Select(id => new Claim("house", id.ToString())));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey)),
                SecurityAlgorithms.HmacSha256),
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
