using WarmHouse.Identity.Domain.Enums;

namespace WarmHouse.Identity.Application.Contracts.Requests;

/// <summary>Grants another user access to a house the caller owns.</summary>
public sealed record GrantAccessRequest(Guid UserId, HouseRole Role);
