using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Domain.Devices;
using WarmHouse.Devices.Domain.Devices.Events;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

/// <summary>
/// Connects a device to a house on the owner's own initiative — the
/// self-service flow that replaces the engineer visit required by the As-Is
/// system.
///
/// After the device is stored, the domain event raised by the aggregate is
/// translated into a <see cref="DeviceRegistered"/> integration event. Domain
/// services subscribe to it and project only the devices they can serve, which
/// is why adding a new device category never requires touching them.
/// </summary>
public sealed class RegisterDeviceHandler(
    IDeviceRepository devices,
    IDeviceTypeRepository deviceTypes,
    IDeviceQueries queries,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<DeviceDto>> HandleAsync(
        RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
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
            request.OwnerId,
            deviceType.Id,
            request.SerialNumber,
            request.Name,
            request.Location,
            request.Firmware,
            clock.UtcNow);

        devices.Add(device);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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

        var dto = await queries.GetDeviceAsync(device.Id, cancellationToken);
        return dto is null ? DeviceErrors.DeviceNotFound : Result<DeviceDto>.Success(dto);
    }
}
