using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts.Responses;

/// <summary>
/// A catalogue entry as the API publishes it. Deliberately separate from the
/// domain entity so that a change to internal modelling does not leak into the
/// published contract.
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
