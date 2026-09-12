using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>A device was removed from a house and must stop being projected.</summary>
public sealed record DeviceDecommissioned(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId) : IIntegrationEvent;
