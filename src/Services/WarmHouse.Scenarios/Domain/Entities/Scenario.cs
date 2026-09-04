using WarmHouse.Scenarios.Domain.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Domain.Entities;

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
