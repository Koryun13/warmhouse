using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Events;

/// <summary>
/// Domain events stay inside the service. The application layer decides which
/// of them are worth announcing to other services as integration events.
/// </summary>
public sealed record DeviceRegisteredDomainEvent(
    DateTimeOffset OccurredAt,
    Guid DeviceId) : IDomainEvent;
