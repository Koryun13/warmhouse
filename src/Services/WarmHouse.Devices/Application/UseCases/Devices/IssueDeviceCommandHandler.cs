using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Domain.Devices;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.Devices;

/// <summary>
/// Accepts a command for a device and hands it to the delivery pipeline.
///
/// The capability is validated against the device's type before anything is
/// queued, so an unsupported action fails immediately with a clear reason
/// instead of timing out somewhere in the transport.
/// </summary>
public sealed class IssueDeviceCommandHandler(
    IDeviceRepository devices,
    IDeviceTypeRepository deviceTypes,
    IDeviceCommandRepository commands,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<DeviceCommandDto>> HandleAsync(
        Guid deviceId,
        IssueCommandRequest request,
        CancellationToken cancellationToken)
    {
        var device = await devices.GetByIdAsync(deviceId, cancellationToken);
        if (device is null || !currentUser.CanAccess(device.HouseId))
        {
            return DeviceErrors.DeviceNotFound;
        }

        var deviceType = await deviceTypes.GetByIdAsync(device.DeviceTypeId, cancellationToken);
        if (deviceType is null)
        {
            return DeviceErrors.DeviceTypeNotFound(device.DeviceTypeId.ToString());
        }

        if (!deviceType.Supports(request.Capability))
        {
            return DeviceErrors.CapabilityNotSupported(request.Capability, deviceType.Capabilities);
        }

        var command = DeviceCommand.Issue(
            device.Id,
            request.Capability,
            request.Action,
            request.Payload,
            currentUser.Id,
            clock.UtcNow);

        commands.Add(command);

        // Delivery is asynchronous: the caller receives 202 and polls the
        // command, rather than holding a connection open while a radio
        // protocol does its work. The event goes to the outbox and leaves it
        // only if the command row is committed with it.
        await publisher.PublishAsync(
            new DeviceCommandRequested(
                Guid.CreateVersion7(),
                clock.UtcNow,
                command.Id,
                command.DeviceId,
                command.Capability,
                command.Action,
                command.Payload,
                command.RequestedBy,
                command.CorrelationId),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(command);
    }

    internal static DeviceCommandDto Map(DeviceCommand command) => new(
        command.Id,
        command.DeviceId,
        command.Capability,
        command.Action,
        command.Payload,
        command.Status,
        command.Error,
        command.CorrelationId,
        command.RequestedAt,
        command.CompletedAt);
}
