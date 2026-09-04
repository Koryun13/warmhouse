using Microsoft.EntityFrameworkCore;
using WarmHouse.Lighting.Domain.Entities;
using WarmHouse.Lighting.Domain.Repositories;

namespace WarmHouse.Lighting.Infrastructure.Persistence.Repositories;

internal sealed class LightFixtureRepository(LightingDbContext context) : ILightFixtureRepository
{
    public Task<LightFixture?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Fixtures.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LightFixture>> ListAsync(Guid? houseId, CancellationToken cancellationToken)
    {
        var query = context.Fixtures.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(f => f.HouseId == house);
        }

        return await query.OrderBy(f => f.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Fixtures.AnyAsync(f => f.DeviceId == deviceId, cancellationToken);

    public void Add(LightFixture fixture) => context.Fixtures.Add(fixture);

    public void RemoveByDevice(Guid deviceId)
        => context.Fixtures.RemoveRange(context.Fixtures.Where(f => f.DeviceId == deviceId));
}
