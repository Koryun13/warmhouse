using WarmHouse.Devices.Application.Contracts.Requests;
using WarmHouse.Devices.Domain.Errors;
using WarmHouse.Devices.Domain.Events;
using WarmHouse.Devices.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Commands;

/// <summary>
/// Records the reachability reported by a device or its gateway. The aggregate
/// decides whether anything actually changed, so no spurious event is published.
/// </summary>
public sealed class UpdateDeviceStatusHandler(
    IDeviceRepository devices,
    ICurrentUser currentUser,
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
        if (device is null || !currentUser.CanAccess(device.HouseId))
        {
            return Result.Failure(DeviceErrors.DeviceNotFound);
        }

        device.ChangeStatus(request.Status, request.Reason, clock.UtcNow);

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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
