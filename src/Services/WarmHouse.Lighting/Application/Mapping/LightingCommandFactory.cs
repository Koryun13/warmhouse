using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Lighting.Application.Mapping;

/// <summary>
/// Builds the device commands this service emits. Centralising the capability
/// names keeps them consistent across the handlers that send them.
/// </summary>
public static class LightingCommandFactory
{
    public static DeviceCommandRequested Command(
        Guid deviceId,
        string capability,
        string action,
        Dictionary<string, string> payload,
        Guid requestedBy,
        DateTimeOffset now)
        => new(Guid.CreateVersion7(), now, Guid.CreateVersion7(), deviceId,
            capability, action, payload, requestedBy, Guid.CreateVersion7());
}
