using WarmHouse.Gates.Application.Contracts.Responses;
using WarmHouse.Gates.Domain.Entities;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Application.Mapping;

/// <summary>Projects the gate aggregate onto the shapes it is published as.</summary>
public static class GateMapper
{
    public static GateDto ToDto(Gate gate) => new(
        gate.Id, gate.HouseId, gate.DeviceId, gate.Name, gate.State,
        gate.SupportsLock, gate.LastOperatedAt, gate.UpdatedAt);

    public static DeviceCommandRequested Command(
        Guid commandId, Guid deviceId, string capability, string action, Guid requestedBy, DateTimeOffset now)
        => new(Guid.CreateVersion7(), now, commandId, deviceId, capability, action,
            new Dictionary<string, string>(), requestedBy, Guid.CreateVersion7());
}
