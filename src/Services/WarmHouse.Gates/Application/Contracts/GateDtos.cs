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

// Gate operations carry no request body: the requester is the authenticated
// caller, taken from the token, and the operation is part of the route.
