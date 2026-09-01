using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Domain.DeviceTypes;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Infrastructure.Persistence.Repositories;

internal sealed class DeviceTypeRepository(DevicesDbContext context) : IDeviceTypeRepository
{
    public Task<DeviceType?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => context.DeviceTypes.FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

    public Task<DeviceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.DeviceTypes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<DeviceType>> ListAsync(
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        var query = context.DeviceTypes.AsNoTracking();

        if (category is { } value)
        {
            query = query.Where(t => t.Category == value);
        }

        return await query.OrderBy(t => t.Code).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(string code, CancellationToken cancellationToken)
        => context.DeviceTypes.AnyAsync(t => t.Code == code, cancellationToken);

    public void Add(DeviceType deviceType) => context.DeviceTypes.Add(deviceType);
}
