using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Application.Mapping;

/// <summary>
/// Projects a device onto its published shape. The catalogue entry is passed in
/// rather than looked up here: code, category and capabilities belong to the
/// type, and reading them per device is what made the As-Is list endpoint N+1.
/// </summary>
public static class DeviceMapper
{
    public static DeviceDto ToDto(Device device, DeviceType type) => new(
        device.Id,
        device.HouseId,
        device.OwnerId,
        device.DeviceTypeId,
        type.Code,
        type.Category,
        device.SerialNumber,
        device.Name,
        device.Location,
        device.Status,
        device.Firmware,
        type.Capabilities,
        device.LastSeenAt,
        device.RegisteredAt);
}
