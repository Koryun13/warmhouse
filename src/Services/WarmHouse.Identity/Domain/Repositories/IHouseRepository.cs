using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Domain.Repositories;

/// <summary>Persistence contract for houses and their membership.</summary>
public interface IHouseRepository
{
    Task<House?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Every house the user can reach, whether as owner or as a member.</summary>
    Task<IReadOnlyList<House>> ListAccessibleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>The same set as <see cref="ListAccessibleAsync"/>, reduced to identifiers for the token.</summary>
    Task<IReadOnlyList<Guid>> ListAccessibleHouseIdsAsync(Guid userId, CancellationToken cancellationToken);

    void Add(House house);
}
