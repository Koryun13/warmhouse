using WarmHouse.Devices.Domain.Devices;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Domain.Abstractions;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Device>> ListAsync(Guid? houseId, DeviceCategory? category, CancellationToken cancellationToken);

    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);

    void Add(Device device);

    void Remove(Device device);
}

public interface IDeviceCommandRepository
{
    Task<DeviceCommand?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DeviceCommand?> GetForDeviceAsync(Guid deviceId, Guid commandId, CancellationToken cancellationToken);

    void Add(DeviceCommand command);
}
