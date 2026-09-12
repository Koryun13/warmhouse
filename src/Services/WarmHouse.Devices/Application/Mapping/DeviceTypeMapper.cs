using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Application.Mapping;

/// <summary>Projects the device type aggregate onto its published shape.</summary>
public static class DeviceTypeMapper
{
    public static DeviceTypeDto ToDto(DeviceType type) => new(
        type.Id,
        type.Code,
        type.Name,
        type.Manufacturer,
        type.Category,
        type.Protocol,
        type.Capabilities,
        type.CreatedAt);
}
