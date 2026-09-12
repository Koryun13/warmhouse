using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Events;

/// <summary>Raised only when a device's reachability actually changes.</summary>
public sealed record DeviceStatusChangedDomainEvent(
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    DeviceStatus PreviousStatus,
    DeviceStatus CurrentStatus,
    string? Reason) : IDomainEvent;
