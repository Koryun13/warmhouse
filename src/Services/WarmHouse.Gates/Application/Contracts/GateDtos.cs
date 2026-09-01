using WarmHouse.Gates.Domain.Gates;

namespace WarmHouse.Gates.Application.Contracts;

public sealed record GateDto(
    Guid Id,
    Guid HouseId,
    Guid DeviceId,
    string Name,
    GateState State,
    bool SupportsLock,
    DateTimeOffset? LastOperatedAt,
    DateTimeOffset UpdatedAt);

public sealed record GateOperationRequest(Guid RequestedBy);
