using WarmHouse.Scenarios.Application.Contracts.Responses;
using WarmHouse.Scenarios.Domain.Entities;

namespace WarmHouse.Scenarios.Application.Mapping;

/// <summary>Translates between the scenario aggregate and its published shapes.</summary>
public static class ScenarioMapper
{
    public static ScenarioStep ToStep(ScenarioStepDto dto) => new()
    {
        Kind = dto.Kind,
        DeviceId = dto.DeviceId,
        Capability = dto.Capability,
        Action = dto.Action,
        RecipientId = dto.RecipientId,
        Channel = dto.Channel,
        Subject = dto.Subject,
        Body = dto.Body,
    };

    public static ScenarioStepDto ToDto(ScenarioStep step) => new(
        step.Kind,
        step.DeviceId,
        step.Capability,
        step.Action,
        step.RecipientId,
        step.Channel,
        step.Subject,
        step.Body);

    public static ScenarioDto ToDto(Scenario scenario) => new(
        scenario.Id,
        scenario.HouseId,
        scenario.Name,
        scenario.Enabled,
        scenario.Trigger,
        scenario.TriggerMetric,
        scenario.TriggerDeviceId,
        [.. scenario.Steps.Select(ToDto)],
        scenario.CreatedAt,
        scenario.LastTriggeredAt);
}
