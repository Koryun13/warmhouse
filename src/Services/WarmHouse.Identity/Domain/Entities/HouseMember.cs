using WarmHouse.Identity.Domain.Enums;

namespace WarmHouse.Identity.Domain.Entities;

/// <summary>Access grant linking a user to a house.</summary>
public sealed class HouseMember
{
    private HouseMember()
    {
        // Required by EF Core.
    }

    internal HouseMember(Guid houseId, Guid userId, HouseRole role, DateTimeOffset grantedAt)
    {
        HouseId = houseId;
        UserId = userId;
        Role = role;
        GrantedAt = grantedAt;
    }

    public Guid HouseId { get; private set; }

    public Guid UserId { get; private set; }

    public HouseRole Role { get; private set; }

    public DateTimeOffset GrantedAt { get; private set; }
}
