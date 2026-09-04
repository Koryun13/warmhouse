using Microsoft.EntityFrameworkCore;
using WarmHouse.Monitoring.Domain.Entities;
using WarmHouse.Monitoring.Domain.Repositories;

namespace WarmHouse.Monitoring.Infrastructure.Persistence.Repositories;

internal sealed class CameraRepository(MonitoringDbContext context) : ICameraRepository
{
    public Task<Camera?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Cameras.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Camera?> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Cameras.FirstOrDefaultAsync(c => c.DeviceId == deviceId, cancellationToken);

    public async Task<IReadOnlyList<Camera>> ListAsync(Guid? houseId, CancellationToken cancellationToken)
    {
        var query = context.Cameras.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(c => c.HouseId == house);
        }

        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Cameras.AnyAsync(c => c.DeviceId == deviceId, cancellationToken);

    public void Add(Camera camera) => context.Cameras.Add(camera);

    public void RemoveByDevice(Guid deviceId)
        => context.Cameras.RemoveRange(context.Cameras.Where(c => c.DeviceId == deviceId));
}
