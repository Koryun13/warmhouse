using Microsoft.EntityFrameworkCore;
using WarmHouse.Heating.Domain.Entities;
using WarmHouse.Heating.Domain.Repositories;

namespace WarmHouse.Heating.Infrastructure.Persistence.Repositories;

internal sealed class HeatingZoneRepository(HeatingDbContext context) : IHeatingZoneRepository
{
    public Task<HeatingZone?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Zones.FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

    public Task<HeatingZone?> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Zones.FirstOrDefaultAsync(z => z.DeviceId == deviceId, cancellationToken);

    public async Task<IReadOnlyList<HeatingZone>> ListAsync(Guid? houseId, CancellationToken cancellationToken)
    {
        var query = context.Zones.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(z => z.HouseId == house);
        }

        return await query.OrderBy(z => z.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Zones.AnyAsync(z => z.DeviceId == deviceId, cancellationToken);

    public void Add(HeatingZone zone) => context.Zones.Add(zone);

    public void RemoveByDevice(Guid deviceId)
        => context.Zones.RemoveRange(context.Zones.Where(z => z.DeviceId == deviceId));
}
