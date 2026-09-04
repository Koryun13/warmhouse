using WarmHouse.Telemetry.Application.Contracts.Responses;
using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Application.Mapping;

/// <summary>Projects the threshold rule aggregate onto its published shape.</summary>
internal static class ThresholdRuleMapper
{
    public static ThresholdRuleDto ToDto(ThresholdRule rule) => new(
        rule.Id, rule.HouseId, rule.DeviceId, rule.Metric,
        rule.OperatorCode, rule.Threshold, rule.Enabled, rule.CreatedAt);
}
