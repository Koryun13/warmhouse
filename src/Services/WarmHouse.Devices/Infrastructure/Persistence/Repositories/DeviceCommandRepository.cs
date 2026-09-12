using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Devices.Domain.Repositories;

namespace WarmHouse.Devices.Infrastructure.Persistence.Repositories;

internal sealed class DeviceCommandRepository(DevicesDbContext context) : IDeviceCommandRepository
{
    public Task<DeviceCommand?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Commands.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<DeviceCommand?> GetForDeviceAsync(
        Guid deviceId,
        Guid commandId,
        CancellationToken cancellationToken)
        => context.Commands.FirstOrDefaultAsync(
            c => c.Id == commandId && c.DeviceId == deviceId, cancellationToken);

    public void Add(DeviceCommand command) => context.Commands.Add(command);
}
