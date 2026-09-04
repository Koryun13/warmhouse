namespace WarmHouse.Shared.Kernel;

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
