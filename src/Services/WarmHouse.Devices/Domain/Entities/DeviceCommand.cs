using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Entities;

/// <summary>
/// A command addressed to a device, together with its delivery outcome.
///
/// It is its own aggregate because its lifecycle (requested, delivered,
/// acknowledged) is independent of the device record it points at.
/// </summary>
public sealed class DeviceCommand : AggregateRoot
{
    private DeviceCommand()
    {
        // Required by EF Core.
    }

    private DeviceCommand(
        Guid id,
        Guid deviceId,
        string capability,
        string action,
        Dictionary<string, string> payload,
        Guid requestedBy,
        Guid correlationId,
        DateTimeOffset requestedAt) : base(id)
    {
        DeviceId = deviceId;
        Capability = capability;
        Action = action;
        Payload = payload;
        RequestedBy = requestedBy;
        CorrelationId = correlationId;
        RequestedAt = requestedAt;
        Status = CommandStatus.Pending;
    }

    public Guid DeviceId { get; private set; }

    public string Capability { get; private set; } = string.Empty;

    public string Action { get; private set; } = string.Empty;

    public Dictionary<string, string> Payload { get; private set; } = [];

    public CommandStatus Status { get; private set; }

    public string? Error { get; private set; }

    public Guid RequestedBy { get; private set; }

    public Guid CorrelationId { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static DeviceCommand Issue(
        Guid deviceId,
        string capability,
        string action,
        Dictionary<string, string>? payload,
        Guid requestedBy,
        DateTimeOffset now,
        Guid? id = null,
        Guid? correlationId = null)
        => new(
            id ?? Guid.CreateVersion7(),
            deviceId,
            capability,
            action,
            payload ?? [],
            requestedBy,
            correlationId ?? Guid.CreateVersion7(),
            now);

    public void MarkAcknowledged(DateTimeOffset now) => Complete(CommandStatus.Acknowledged, null, now);

    public void MarkFailed(string error, DateTimeOffset now) => Complete(CommandStatus.Failed, error, now);

    private void Complete(CommandStatus status, string? error, DateTimeOffset now)
    {
        Status = status;
        Error = error;
        CompletedAt = now;
    }
}
