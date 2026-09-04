using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Application.Mapping;

/// <summary>Projects the command aggregate onto its published shape.</summary>
public static class DeviceCommandMapper
{
    public static DeviceCommandDto ToDto(DeviceCommand command) => new(
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
