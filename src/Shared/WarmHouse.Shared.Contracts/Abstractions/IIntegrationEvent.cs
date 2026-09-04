namespace WarmHouse.Shared.Contracts.Abstractions;

/// <summary>
/// Public contract exchanged between services over the message broker.
/// This assembly is the only thing services share about each other, so it is
/// kept free of any behaviour or infrastructure concern.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}
