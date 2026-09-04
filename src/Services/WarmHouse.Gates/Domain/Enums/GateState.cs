namespace WarmHouse.Gates.Domain.Enums;

/// <summary>
/// Position of a gate leaf. The transitional states are modelled explicitly
/// because operating a gate is not instantaneous.
/// </summary>
public enum GateState
{
    Closed = 0,
    Opening = 1,
    Open = 2,
    Closing = 3,
    Locked = 4,

    /// <summary>The leaf position is not known, e.g. after a failed command.</summary>
    Unknown = 5,
}
