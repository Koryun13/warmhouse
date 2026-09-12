using WarmHouse.Devices.Application.Contracts.Requests;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Application.Mapping;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Devices.Domain.Errors;
using WarmHouse.Devices.Domain.Events;
using WarmHouse.Devices.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Commands;

/// <summary>
/// Connects a device to a house on the owner's own initiative — the
/// self-service flow that replaces the engineer visit required by the As-Is
/// system.
///
/// The domain event raised by the aggregate is translated into a
/// <see cref="DeviceRegistered"/> integration event. Domain services subscribe
/// to it and project only the devices they can serve, which is why adding a new
/// device category never requires touching them.
///
/// The publish precedes the save on purpose: it puts the event in the outbox
/// table, and the same transaction that stores the device commits it. Either
/// both are durable or neither is.
/// </summary>
public sealed class RegisterDeviceHandler(
    IDeviceRepository devices,
    IDeviceTypeRepository deviceTypes,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<DeviceDto>> HandleAsync(
        RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(request.HouseId))
        {
            return AccessErrors.HouseForbidden(request.HouseId);
        }

        var deviceType = await deviceTypes.GetByCodeAsync(request.DeviceTypeCode, cancellationToken);
        if (deviceType is null)
        {
            return DeviceErrors.DeviceTypeNotFound(request.DeviceTypeCode);
        }

        if (await devices.SerialNumberExistsAsync(request.SerialNumber, cancellationToken))
        {
            return DeviceErrors.SerialNumberTaken(request.SerialNumber);
        }

        var device = Device.Register(
            request.HouseId,
            currentUser.Id,
            deviceType.Id,
            request.SerialNumber,
            request.Name,
            request.Location,
            request.Firmware,
            clock.UtcNow);

        devices.Add(device);

        foreach (var _ in device.DomainEvents.OfType<DeviceRegisteredDomainEvent>())
        {
            await publisher.PublishAsync(
                new DeviceRegistered(
                    Guid.CreateVersion7(),
                    clock.UtcNow,
                    device.Id,
                    device.HouseId,
                    device.OwnerId,
                    deviceType.Code,
                    deviceType.Category,
                    device.SerialNumber,
                    deviceType.Capabilities,
                    deviceType.Protocol),
                cancellationToken);
        }

        device.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return DeviceMapper.ToDto(device, deviceType);
    }
}
