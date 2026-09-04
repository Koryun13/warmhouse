namespace WarmHouse.Shared.Contracts.Enums;

/// <summary>Lifecycle of a command sent towards a device.</summary>
public enum CommandStatus
{
    Pending = 0,
    Sent = 1,
    Acknowledged = 2,
    Failed = 3,
    TimedOut = 4,
}
