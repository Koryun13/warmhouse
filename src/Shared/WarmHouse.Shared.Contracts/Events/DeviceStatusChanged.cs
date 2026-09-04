using WarmHouse.Shared.Contracts.Abstractions;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>Device reachability changed. Device management owns this truth.</summary>
public sealed record DeviceStatusChanged(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    DeviceStatus PreviousStatus,
    DeviceStatus CurrentStatus,
    string? Reason) : IIntegrationEvent;
