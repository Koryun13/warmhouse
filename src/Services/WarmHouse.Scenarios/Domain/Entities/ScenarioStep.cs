using WarmHouse.Scenarios.Domain.Enums;

namespace WarmHouse.Scenarios.Domain.Entities;

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
