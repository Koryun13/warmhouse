using Microsoft.EntityFrameworkCore;
using WarmHouse.Identity.Domain.Abstractions;
using WarmHouse.Identity.Domain.Houses;
using WarmHouse.Identity.Domain.Users;

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

internal sealed class HouseRepository(IdentityDbContext context) : IHouseRepository
{
    public Task<House?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Houses.Include(h => h.Members).FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public async Task<IReadOnlyList<House>> ListAsync(Guid? ownerId, CancellationToken cancellationToken)
    {
        var query = context.Houses.AsNoTracking();

        if (ownerId is { } owner)
        {
            query = query.Where(h => h.OwnerId == owner);
        }

        return await query.OrderBy(h => h.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> ListAccessibleHouseIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
        => await context.HouseMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .Select(m => m.HouseId)
            .Distinct()
            .ToListAsync(cancellationToken);

    public void Add(House house) => context.Houses.Add(house);
}
