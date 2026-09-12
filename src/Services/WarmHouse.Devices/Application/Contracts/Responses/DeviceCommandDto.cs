using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts.Responses;

/// <summary>A command and its delivery outcome, as the API publishes it.</summary>
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
