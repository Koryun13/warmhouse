using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WarmHouse.Shared.Application.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Security;

/// <summary>
/// Token validation shared by every service that exposes protected resources.
///
/// The identity service issues the token; every other service validates it
/// locally against the same signing key, so authorisation costs no network hop.
/// Authorisation is deny by default: an endpoint is reachable without a token
/// only when it opts out with <c>AllowAnonymous</c>.
/// </summary>
public static class AuthenticationSetup
{
    /// <summary>Claim carrying one house the token grants access to. Repeated per house.</summary>
    public const string HouseClaim = "house";

    /// <summary>Development fallback only; a deployment must supply JWT_SIGNING_KEY.</summary>
    public const string DevelopmentSigningKey = "dev-only-signing-key-change-me-in-production-32b";

    public static WebApplicationBuilder AddServiceAuthentication(this WebApplicationBuilder builder)
    {
        var issuer = builder.Configuration["JWT_ISSUER"] ?? "warmhouse-identity";
        var audience = builder.Configuration["JWT_AUDIENCE"] ?? "warmhouse";
        var signingKey = builder.Configuration["JWT_SIGNING_KEY"] ?? DevelopmentSigningKey;

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Keep the claim types the issuer wrote ("sub" stays "sub")
                // instead of the legacy WS-Federation mapping.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Name,
                };
            });

        builder.Services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

        return builder;
    }
}
