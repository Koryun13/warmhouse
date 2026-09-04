using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using WarmHouse.Shared.Application.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Security;

/// <summary>
/// Reads the caller out of the validated token. Scoped, so the house claims are
/// parsed once per request.
/// </summary>
internal sealed class HttpContextCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private Guid[]? _houseIds;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid Id =>
        Guid.TryParse(Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : Guid.Empty;

    public IReadOnlyCollection<Guid> HouseIds => _houseIds ??= ReadHouseIds();

    public bool CanAccess(Guid houseId) => houseId != Guid.Empty && HouseIds.Contains(houseId);

    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    private Guid[] ReadHouseIds()
    {
        var principal = Principal;
        if (principal is null)
        {
            return [];
        }

        var houses = new List<Guid>();
        foreach (var claim in principal.FindAll(AuthenticationSetup.HouseClaim))
        {
            if (Guid.TryParse(claim.Value, out var houseId))
            {
                houses.Add(houseId);
            }
        }

        return [.. houses];
    }
}
