using Microsoft.EntityFrameworkCore;
using WarmHouse.Identity.Domain.Entities;
using WarmHouse.Identity.Domain.Repositories;

namespace WarmHouse.Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        => context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
}
