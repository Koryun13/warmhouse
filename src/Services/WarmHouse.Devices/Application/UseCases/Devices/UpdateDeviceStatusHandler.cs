using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Domain.Devices.Events;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

/// <summary>
/// Records the reachability reported by a device or its gateway. The aggregate
/// decides whether anything actually changed, so no spurious event is published.
/// </summary>
public sealed class UpdateDeviceStatusHandler(
    IDeviceRepository devices,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result> HandleAsync(
        Guid deviceId,
        UpdateDeviceStatusRequest request,
        CancellationToken cancellationToken)
    {
        var device = await devices.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return Result.Failure(DeviceErrors.DeviceNotFound);
        }

        device.ChangeStatus(request.Status, request.Reason, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var change in device.DomainEvents.OfType<DeviceStatusChangedDomainEvent>())
        {
            await publisher.PublishAsync(
                new DeviceStatusChanged(
                    Guid.CreateVersion7(),
                    clock.UtcNow,
                    change.DeviceId,
                    change.PreviousStatus,
                    change.CurrentStatus,
                    change.Reason),
                cancellationToken);
        }

        device.ClearDomainEvents();
        return Result.Success();
    }
}
