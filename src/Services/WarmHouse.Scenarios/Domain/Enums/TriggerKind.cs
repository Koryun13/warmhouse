namespace WarmHouse.Scenarios.Domain.Enums;

/// <summary>What makes a scenario fire.</summary>
public enum TriggerKind
{
    /// <summary>Fires when a metric crosses a threshold.</summary>
    TelemetryThreshold = 0,

    /// <summary>Fires when a device changes reachability.</summary>
    DeviceStatus = 1,
}
