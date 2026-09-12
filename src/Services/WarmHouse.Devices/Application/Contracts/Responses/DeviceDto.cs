using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts.Responses;

/// <summary>A connected device as the API publishes it.</summary>
public sealed record DeviceDto(
    Guid Id,
    Guid HouseId,
    Guid OwnerId,
    Guid DeviceTypeId,
    string DeviceTypeCode,
    DeviceCategory Category,
    string SerialNumber,
    string Name,
    string? Location,
    DeviceStatus Status,
    string? Firmware,
    IReadOnlyCollection<string> Capabilities,
    DateTimeOffset? LastSeenAt,
    DateTimeOffset RegisteredAt);
