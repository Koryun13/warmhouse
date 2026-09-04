using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Errors;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Queries;

public sealed class GetDeviceHandler(IDeviceQueries queries, ICurrentUser currentUser)
{
    public async Task<Result<DeviceDto>> HandleAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = await queries.GetDeviceAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return DeviceErrors.DeviceNotFound;
        }

        // Not found rather than forbidden: whether a device exists in someone
        // else's house is not the caller's business.
        return currentUser.CanAccess(device.HouseId)
            ? Result<DeviceDto>.Success(device)
            : DeviceErrors.DeviceNotFound;
    }
}
