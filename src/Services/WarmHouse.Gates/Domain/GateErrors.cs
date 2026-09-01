using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Domain;

public static class GateErrors
{
    public static Error NotFound => Error.NotFound("gate.not_found", "Gate not found");

    public static Error Locked => Error.Conflict(
        "gate.locked",
        "Gate is locked",
        "Unlock the gate before operating it.");

    public static Error LockNotSupported => Error.Unprocessable(
        "gate.lock_unsupported",
        "Locking is not supported",
        "The drive does not declare the gate.lock capability.");

    public static Error NotClosed(string state) => Error.Conflict(
        "gate.not_closed",
        "Gate is not closed",
        $"Only a closed gate can be locked; the current state is {state}.");
}
