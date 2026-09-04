using WarmHouse.Devices.Domain.Events;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Entities;

/// <summary>
/// A physical device a user has connected to their house.
///
/// State transitions live here rather than in a use case, so that the rule
/// "only a real change is announced" cannot be bypassed by a caller.
/// </summary>
public sealed class Device : AggregateRoot
{
    private Device()
    {
        // Required by EF Core.
    }

    private Device(
        Guid id,
        Guid houseId,
        Guid ownerId,
        Guid deviceTypeId,
        string serialNumber,
        string name,
        string? location,
        string? firmware,
        DateTimeOffset registeredAt) : base(id)
    {
        HouseId = houseId;
        OwnerId = ownerId;
        DeviceTypeId = deviceTypeId;
        SerialNumber = serialNumber;
        Name = name;
        Location = location;
        Firmware = firmware;
        RegisteredAt = registeredAt;
        LastSeenAt = registeredAt;
        Status = DeviceStatus.Online;
    }

    public Guid HouseId { get; private set; }

    public Guid OwnerId { get; private set; }

    public Guid DeviceTypeId { get; private set; }

    public string SerialNumber { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    /// <summary>Room or zone inside the house.</summary>
    public string? Location { get; private set; }

    public DeviceStatus Status { get; private set; } = DeviceStatus.Offline;

    public string? Firmware { get; private set; }

    public DateTimeOffset? LastSeenAt { get; private set; }

    public DateTimeOffset RegisteredAt { get; private set; }

    public static Device Register(
        Guid houseId,
        Guid ownerId,
        Guid deviceTypeId,
        string serialNumber,
        string name,
        string? location,
        string? firmware,
        DateTimeOffset now)
    {
        var device = new Device(
            Guid.CreateVersion7(),
            houseId,
            ownerId,
            deviceTypeId,
            serialNumber.Trim(),
            name.Trim(),
            location?.Trim(),
            firmware?.Trim(),
            now);

        device.Raise(new DeviceRegisteredDomainEvent(now, device.Id));
        return device;
    }

    /// <summary>
    /// Records a new reachability state. A domain event is raised only when the
    /// status actually changes, which keeps subscribers free of duplicates.
    /// </summary>
    public void ChangeStatus(DeviceStatus status, string? reason, DateTimeOffset now)
    {
        if (Status == status)
        {
            LastSeenAt = now;
            return;
        }

        var previous = Status;
        Status = status;
        LastSeenAt = now;

        Raise(new DeviceStatusChangedDomainEvent(now, Id, previous, status, reason));
    }

    public void MarkSeen(DateTimeOffset now) => LastSeenAt = now;

    /// <summary>A device that is offline or faulted cannot accept commands.</summary>
    public bool CanAcceptCommands => Status is DeviceStatus.Online or DeviceStatus.Maintenance;
}
