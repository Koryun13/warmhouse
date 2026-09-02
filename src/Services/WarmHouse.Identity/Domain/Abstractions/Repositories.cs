using WarmHouse.Identity.Domain.Houses;
using WarmHouse.Identity.Domain.Users;

namespace WarmHouse.Identity.Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);

    void Add(User user);
}

public interface IHouseRepository
{
    Task<House?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Every house the user can reach, whether as owner or as a member.</summary>
    Task<IReadOnlyList<House>> ListAccessibleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>The same set as <see cref="ListAccessibleAsync"/>, reduced to identifiers for the token.</summary>
    Task<IReadOnlyList<Guid>> ListAccessibleHouseIdsAsync(Guid userId, CancellationToken cancellationToken);

    void Add(House house);
}
