using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Queries;

/// <summary>
/// Lists the devices of one house, optionally narrowed to one category.
///
/// The house is required: a list endpoint that returned every device when the
/// filter was omitted would leak the whole registry to any authenticated caller.
/// </summary>
public sealed class ListDevicesHandler(IDeviceQueries queries, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<DeviceDto>>> HandleAsync(
        Guid houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        return Result<IReadOnlyList<DeviceDto>>.Success(
            await queries.ListDevicesAsync(houseId, category, cancellationToken));
    }
}
