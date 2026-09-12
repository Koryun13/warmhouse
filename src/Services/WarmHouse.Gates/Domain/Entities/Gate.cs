using WarmHouse.Gates.Domain.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Domain.Entities;

/// <summary>
/// An automatic gate.
///
/// Operating a gate is not instantaneous, so the aggregate models the
/// transitional states explicitly and remembers which command it is waiting
/// for. The final state is only reached when the drive acknowledges.
/// </summary>
public sealed class Gate : AggregateRoot
{
    private Gate()
    {
        // Required by EF Core.
    }

    private Gate(Guid id, Guid houseId, Guid deviceId, string name, bool supportsLock, DateTimeOffset now)
        : base(id)
    {
        HouseId = houseId;
        DeviceId = deviceId;
        Name = name;
        SupportsLock = supportsLock;
        State = GateState.Closed;
        UpdatedAt = now;
    }

    public Guid HouseId { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public GateState State { get; private set; }

    public bool SupportsLock { get; private set; }

    public DateTimeOffset? LastOperatedAt { get; private set; }

    /// <summary>The command whose acknowledgement will settle the state.</summary>
    public Guid? PendingCommandId { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Gate ForDevice(
        Guid houseId, Guid deviceId, string name, bool supportsLock, DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, deviceId, name, supportsLock, now);

    public bool IsLocked => State == GateState.Locked;

    public bool CanLock => State == GateState.Closed;

    public void BeginOperation(Guid commandId, GateState transitionalState, DateTimeOffset now)
    {
        State = transitionalState;
        PendingCommandId = commandId;
        LastOperatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Settles the state once the drive reports back. A failed command leaves
    /// the gate in <see cref="GateState.Unknown"/> rather than guessing, because
    /// the real position of the leaf cannot be assumed.
    /// </summary>
    public void SettleOperation(Guid commandId, bool acknowledged, DateTimeOffset now)
    {
        if (PendingCommandId != commandId)
        {
            return;
        }

        State = acknowledged
            ? State switch
            {
                GateState.Opening => GateState.Open,
                GateState.Closing => GateState.Closed,
                GateState.Locked => GateState.Locked,
                _ => State,
            }
            : GateState.Unknown;

        PendingCommandId = null;
        UpdatedAt = now;
    }
}
