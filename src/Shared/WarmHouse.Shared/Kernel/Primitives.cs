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

/// <summary>Base class for entities identified by a <see cref="Guid"/>.</summary>
public abstract class Entity
{
    protected Entity(Guid id) => Id = id;

    /// <summary>Required by EF Core materialisation.</summary>
    protected Entity()
    {
    }

    public Guid Id { get; protected set; }

    public override bool Equals(object? obj)
        => obj is Entity other && other.GetType() == GetType() && other.Id == Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}

/// <summary>
/// Consistency boundary. Only aggregate roots are loaded and saved as a whole,
/// and only they raise domain events.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
