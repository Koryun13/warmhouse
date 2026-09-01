using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.DeviceTypes;

/// <summary>Returns the catalogue of device classes the ecosystem supports.</summary>
public sealed class ListDeviceTypesHandler(IDeviceQueries queries)
{
    public async Task<Result<IReadOnlyList<DeviceTypeDto>>> HandleAsync(
        DeviceCategory? category,
        CancellationToken cancellationToken)
        => Result<IReadOnlyList<DeviceTypeDto>>.Success(
            await queries.ListDeviceTypesAsync(category, cancellationToken));
}
