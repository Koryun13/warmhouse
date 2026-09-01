using WarmHouse.Devices.Domain;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

/// <summary>Removes a device and lets the domain services drop their projections.</summary>
public sealed class DecommissionDeviceHandler(
    IDeviceRepository devices,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = await devices.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return Result.Failure(DeviceErrors.DeviceNotFound);
        }

        var houseId = device.HouseId;
        devices.Remove(device);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            new DeviceDecommissioned(Guid.CreateVersion7(), clock.UtcNow, deviceId, houseId),
            cancellationToken);

        return Result.Success();
    }
}
