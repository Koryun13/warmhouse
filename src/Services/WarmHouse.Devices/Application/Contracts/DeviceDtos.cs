using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts;

/// <summary>
/// Shapes returned by the use cases. They are deliberately separate from the
/// domain entities so that a change to internal modelling does not leak into
/// the published API contract.
/// </summary>
public sealed record DeviceTypeDto(
    Guid Id,
    string Code,
    string Name,
    string Manufacturer,
    DeviceCategory Category,
    ConnectivityProtocol Protocol,
    IReadOnlyCollection<string> Capabilities,
    DateTimeOffset CreatedAt);

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

public sealed record DeviceCommandDto(
    Guid Id,
    Guid DeviceId,
    string Capability,
    string Action,
    IReadOnlyDictionary<string, string> Payload,
    CommandStatus Status,
    string? Error,
    Guid CorrelationId,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt);
