using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

public sealed class GetCommandStatusHandler(IDeviceQueries queries)
{
    public async Task<Result<DeviceCommandDto>> HandleAsync(
        Guid deviceId,
        Guid commandId,
        CancellationToken cancellationToken)
    {
        var command = await queries.GetCommandAsync(deviceId, commandId, cancellationToken);
        return command is null ? DeviceErrors.CommandNotFound : Result<DeviceCommandDto>.Success(command);
    }
}
