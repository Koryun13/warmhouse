using WarmHouse.Identity.Domain.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Domain.Entities;

/// <summary>
/// A home in the ecosystem.
///
/// A house has exactly one owner, but access can be granted to several users
/// through <see cref="HouseMember"/> — that is the difference between the
/// one-to-many ownership link and the many-to-many access link.
/// </summary>
public sealed class House : AggregateRoot
{
    public const string DefaultTimeZone = "Asia/Almaty";

    private readonly List<HouseMember> _members = [];

    private House()
    {
        // Required by EF Core.
    }

    private House(
        Guid id, Guid ownerId, string name, string? address, string timeZone, DateTimeOffset now) : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Address = address;
        TimeZone = timeZone;
        CreatedAt = now;

        // The owner is always a member, so access checks need one rule only.
        _members.Add(new HouseMember(id, ownerId, HouseRole.Owner, now));
    }

    public Guid OwnerId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Address { get; private set; }

    public string TimeZone { get; private set; } = DefaultTimeZone;

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<HouseMember> Members => _members.AsReadOnly();

    public static House Create(
        Guid ownerId, string name, string? address, string? timeZone, DateTimeOffset now)
        => new(
            Guid.CreateVersion7(),
            ownerId,
            name.Trim(),
            address?.Trim(),
            string.IsNullOrWhiteSpace(timeZone) ? DefaultTimeZone : timeZone.Trim(),
            now);

    public bool HasMember(Guid userId) => _members.Any(m => m.UserId == userId);

    public void GrantAccess(Guid userId, HouseRole role, DateTimeOffset now)
        => _members.Add(new HouseMember(Id, userId, role, now));
}
