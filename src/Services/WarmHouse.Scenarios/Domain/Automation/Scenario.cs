using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Domain.Automation;

public enum TriggerKind
{
    /// <summary>Fires when a metric crosses a threshold.</summary>
    TelemetryThreshold = 0,

    /// <summary>Fires when a device changes reachability.</summary>
    DeviceStatus = 1,
}

public enum ActionKind
{
    DeviceCommand = 0,
    Notify = 1,
}

/// <summary>
/// One step of a scenario. Steps are stored as a document inside the scenario
/// because they are always read and rewritten together with it.
/// </summary>
public sealed class ScenarioStep
{
    public ActionKind Kind { get; set; } = ActionKind.DeviceCommand;

    public Guid? DeviceId { get; set; }

    public string? Capability { get; set; }

    public string? Action { get; set; }

    public Guid? RecipientId { get; set; }

    public string? Channel { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    /// <summary>A step is only executable if it carries the fields its kind needs.</summary>
    public bool IsExecutable => Kind switch
    {
        ActionKind.DeviceCommand => DeviceId is not null && !string.IsNullOrWhiteSpace(Capability),
        ActionKind.Notify => RecipientId is not null,
        _ => false,
    };
}

/// <summary>
/// A user-defined automation rule.
///
/// This is the "buyers can programme the system to their own needs"
/// requirement: a household composes triggers and actions without anyone
/// writing code for their particular combination.
/// </summary>
public sealed class Scenario : AggregateRoot
{
    private readonly List<ScenarioStep> _steps = [];

    private Scenario()
    {
        // Required by EF Core.
    }

    private Scenario(
        Guid id,
        Guid houseId,
        string name,
        TriggerKind triggerKind,
        string? triggerMetric,
        Guid? triggerDeviceId,
        IEnumerable<ScenarioStep> steps,
        DateTimeOffset now) : base(id)
    {
        HouseId = houseId;
        Name = name;
        Trigger = triggerKind;
        TriggerMetric = triggerMetric?.Trim().ToLowerInvariant();
        TriggerDeviceId = triggerDeviceId;
        Enabled = true;
        CreatedAt = now;
        _steps.AddRange(steps);
    }

    public Guid HouseId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool Enabled { get; private set; }

    public TriggerKind Trigger { get; private set; }

    public string? TriggerMetric { get; private set; }

    /// <summary>Null means the scenario watches every device in the house.</summary>
    public Guid? TriggerDeviceId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? LastTriggeredAt { get; private set; }

    public IReadOnlyCollection<ScenarioStep> Steps => _steps.AsReadOnly();

    public static Scenario Create(
        Guid houseId,
        string name,
        TriggerKind triggerKind,
        string? triggerMetric,
        Guid? triggerDeviceId,
        IEnumerable<ScenarioStep> steps,
        DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, name.Trim(), triggerKind,
            triggerMetric, triggerDeviceId, steps, now);

    public void SetEnabled(bool enabled) => Enabled = enabled;

    public void MarkTriggered(DateTimeOffset now) => LastTriggeredAt = now;
}
