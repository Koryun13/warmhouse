using Microsoft.EntityFrameworkCore;
using WarmHouse.Identity.Domain.Entities;
using WarmHouse.Identity.Domain.Repositories;

namespace WarmHouse.Identity.Infrastructure.Persistence.Repositories;

internal sealed class HouseRepository(IdentityDbContext context) : IHouseRepository
{
    public Task<House?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Houses.Include(h => h.Members).FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public async Task<IReadOnlyList<House>> ListAccessibleAsync(
        Guid userId,
        CancellationToken cancellationToken)
        => await context.Houses
            .AsNoTracking()
            .Where(h => context.HouseMembers.Any(m => m.HouseId == h.Id && m.UserId == userId))
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

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
