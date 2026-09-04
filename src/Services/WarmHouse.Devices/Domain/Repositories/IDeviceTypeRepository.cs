using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Domain.Repositories;

/// <summary>
/// Persistence contracts owned by the domain. The application layer depends on
/// these; the concrete EF Core implementations live in the infrastructure layer,
/// so the dependency always points inwards.
/// </summary>
public interface IDeviceTypeRepository
{
    Task<DeviceType?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    Task<DeviceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<DeviceType>> ListAsync(DeviceCategory? category, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken);

    void Add(DeviceType deviceType);
}
