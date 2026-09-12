using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Domain.Repositories;

/// <summary>Persistence contract for issued commands and their outcomes.</summary>
public interface IDeviceCommandRepository
{
    Task<DeviceCommand?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DeviceCommand?> GetForDeviceAsync(Guid deviceId, Guid commandId, CancellationToken cancellationToken);

    void Add(DeviceCommand command);
}
