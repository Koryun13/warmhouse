using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Devices.Domain.Repositories;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Infrastructure.Persistence.Repositories;

internal sealed class DeviceRepository(DevicesDbContext context) : IDeviceRepository
{
    public Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Devices.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Device>> ListAsync(
        Guid? houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        var query = context.Devices.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(d => d.HouseId == house);
        }

        if (category is { } value)
        {
            query = query.Where(d => context.DeviceTypes
                .Any(t => t.Id == d.DeviceTypeId && t.Category == value));
        }

        return await query.OrderBy(d => d.RegisteredAt).ToListAsync(cancellationToken);
    }

    public Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
        => context.Devices.AnyAsync(d => d.SerialNumber == serialNumber, cancellationToken);

    public void Add(Device device) => context.Devices.Add(device);

    public void Remove(Device device) => context.Devices.Remove(device);
}
