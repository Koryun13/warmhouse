using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Devices.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Devices.Application.Handlers.Events;

/// <summary>
/// Delivers a requested command to its device and reports the outcome.
///
/// The request may come from this service's own API or from a domain service
/// or scenario that never talks to the device directly. Either way, the
/// command is recorded here first, so the reachability and capability checks
/// happen in exactly one place.
/// </summary>
public sealed class DeliverDeviceCommandHandler(
    IDeviceRepository devices,
    IDeviceTypeRepository deviceTypes,
    IDeviceCommandRepository commands,
    IDeviceGateway gateway,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task HandleAsync(DeviceCommandRequested message, CancellationToken cancellationToken)
    {
        var device = await devices.GetByIdAsync(message.DeviceId, cancellationToken);
        var command = await commands.GetByIdAsync(message.CommandId, cancellationToken);

        // A command issued by another service has no record here yet.
        if (command is null && device is not null)
        {
            command = DeviceCommand.Issue(
                message.DeviceId,
                message.Capability,
                message.Action,
                new Dictionary<string, string>(message.Payload),
                message.RequestedBy,
                message.OccurredAt,
                message.CommandId,
                message.CorrelationId);

            commands.Add(command);
        }

        if (device is null)
        {
            await CompleteAsync(command, CommandStatus.Failed, "Device not found", message, cancellationToken);
            return;
        }

        if (!device.CanAcceptCommands)
        {
            await CompleteAsync(
                command, CommandStatus.Failed, $"Device is {device.Status}", message, cancellationToken);
            return;
        }

        var deviceType = await deviceTypes.GetByIdAsync(device.DeviceTypeId, cancellationToken);
        if (deviceType is null || !deviceType.Supports(message.Capability))
        {
            await CompleteAsync(
                command,
                CommandStatus.Failed,
                $"Capability '{message.Capability}' is not supported",
                message,
                cancellationToken);
            return;
        }

        var delivered = await gateway.SendAsync(
            device, deviceType.Protocol, message.Capability, message.Action, message.Payload, cancellationToken);

        device.MarkSeen(clock.UtcNow);

        await CompleteAsync(
            command,
            delivered ? CommandStatus.Acknowledged : CommandStatus.Failed,
            delivered ? null : "The device gateway rejected the command",
            message,
            cancellationToken);
    }

    private async Task CompleteAsync(
        DeviceCommand? command,
        CommandStatus status,
        string? error,
        DeviceCommandRequested message,
        CancellationToken cancellationToken)
    {
        if (command is not null)
        {
            if (status == CommandStatus.Acknowledged)
            {
                command.MarkAcknowledged(clock.UtcNow);
            }
            else
            {
                command.MarkFailed(error ?? "Delivery failed", clock.UtcNow);
            }
        }

        await publisher.PublishAsync(
            new DeviceCommandCompleted(
                Guid.CreateVersion7(),
                clock.UtcNow,
                message.CommandId,
                message.DeviceId,
                status,
                error,
                message.CorrelationId),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
