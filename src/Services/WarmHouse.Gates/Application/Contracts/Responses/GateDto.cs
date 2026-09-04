using WarmHouse.Gates.Domain.Enums;

namespace WarmHouse.Gates.Application.Contracts.Responses;

/// <summary>A gate as the API publishes it.</summary>
public sealed record GateDto(
    Guid Id,
    Guid HouseId,
    Guid DeviceId,
    string Name,
    GateState State,
    bool SupportsLock,
    DateTimeOffset? LastOperatedAt,
    DateTimeOffset UpdatedAt);
