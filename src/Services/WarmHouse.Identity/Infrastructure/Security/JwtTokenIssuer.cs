using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Domain.Users;
using WarmHouse.Shared.Infrastructure.Security;

namespace WarmHouse.Identity.Infrastructure.Security;

/// <summary>Issues signed JWTs carrying the houses a user may reach.</summary>
internal sealed class JwtTokenIssuer(IConfiguration configuration) : ITokenIssuer
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(8);

    private readonly string _issuer = configuration["JWT_ISSUER"] ?? "warmhouse-identity";
    private readonly string _audience = configuration["JWT_AUDIENCE"] ?? "warmhouse";

    // The issuer and the services that validate the token read the same
    // settings, so a mismatch cannot be introduced on one side only.
    private readonly string _signingKey = configuration["JWT_SIGNING_KEY"]
        ?? AuthenticationSetup.DevelopmentSigningKey;

    public (string Token, DateTimeOffset ExpiresAt) Issue(User user, IReadOnlyCollection<Guid> houseIds)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(Lifetime);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.DisplayName),
        };

        claims.AddRange(houseIds.Select(id => new Claim(AuthenticationSetup.HouseClaim, id.ToString())));

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
