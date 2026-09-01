using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

/// <summary>Lists the devices of a house, optionally narrowed to one category.</summary>
public sealed class ListDevicesHandler(IDeviceQueries queries)
{
    public async Task<Result<IReadOnlyList<DeviceDto>>> HandleAsync(
        Guid? houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken)
        => Result<IReadOnlyList<DeviceDto>>.Success(
            await queries.ListDevicesAsync(houseId, category, cancellationToken));
}
