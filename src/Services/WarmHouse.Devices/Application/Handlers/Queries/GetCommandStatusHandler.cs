using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Errors;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Queries;

public sealed class GetCommandStatusHandler(IDeviceQueries queries, ICurrentUser currentUser)
{
    public async Task<Result<DeviceCommandDto>> HandleAsync(
        Guid deviceId,
        Guid commandId,
        CancellationToken cancellationToken)
    {
        var device = await queries.GetDeviceAsync(deviceId, cancellationToken);
        if (device is null || !currentUser.CanAccess(device.HouseId))
        {
            return DeviceErrors.DeviceNotFound;
        }

        var command = await queries.GetCommandAsync(deviceId, commandId, cancellationToken);
        return command is null ? DeviceErrors.CommandNotFound : Result<DeviceCommandDto>.Success(command);
    }
}
