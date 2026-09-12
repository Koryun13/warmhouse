namespace WarmHouse.Shared.Kernel;

/// <summary>
/// Something that happened inside an aggregate. Domain events never leave the
/// service: the application layer translates the ones that matter to the
/// outside world into integration events.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
