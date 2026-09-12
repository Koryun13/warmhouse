using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Domain.Repositories;

/// <summary>Persistence contract for accounts.</summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);

    void Add(User user);
}
