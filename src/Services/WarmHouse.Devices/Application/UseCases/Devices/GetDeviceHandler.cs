using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

public sealed class GetDeviceHandler(IDeviceQueries queries)
{
    public async Task<Result<DeviceDto>> HandleAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = await queries.GetDeviceAsync(deviceId, cancellationToken);
        return device is null ? DeviceErrors.DeviceNotFound : Result<DeviceDto>.Success(device);
    }
}
