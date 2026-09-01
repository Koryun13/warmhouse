using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Abstractions;

/// <summary>
/// Read side of the service. Queries project straight to DTOs in a single
/// database round trip, which keeps list endpoints free of the N+1 problem the
/// original monolith suffered from.
/// </summary>
public interface IDeviceQueries
{
    Task<IReadOnlyList<DeviceTypeDto>> ListDeviceTypesAsync(
        DeviceCategory? category,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DeviceDto>> ListDevicesAsync(
        Guid? houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken);

    Task<DeviceDto?> GetDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    Task<DeviceCommandDto?> GetCommandAsync(Guid deviceId, Guid commandId, CancellationToken cancellationToken);
}
