using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Domain.Repositories;

/// <summary>
/// Persistence contract owned by the domain. The application layer depends on
/// this; the concrete EF Core implementation lives in the infrastructure layer,
/// so the dependency always points inwards.
/// </summary>
public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Device>> ListAsync(Guid? houseId, DeviceCategory? category, CancellationToken cancellationToken);

    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);

    void Add(Device device);

    void Remove(Device device);
}
