using WarmHouse.Shared.Contracts.Abstractions;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>
/// A user connected a device through self-service.
///
/// Domain services subscribe to this and project only the devices they can
/// serve, matching on category or on a declared capability. Unknown categories
/// are ignored, so a brand new device class needs no change to existing code.
/// </summary>
public sealed record DeviceRegistered(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId,
    Guid OwnerId,
    string DeviceTypeCode,
    DeviceCategory Category,
    string SerialNumber,
    IReadOnlyCollection<string> Capabilities,
    ConnectivityProtocol Protocol) : IIntegrationEvent;
