using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Application.Contracts.Responses;
using WarmHouse.Telemetry.Application.Mapping;
using WarmHouse.Telemetry.Domain.Entities;
using WarmHouse.Telemetry.Domain.Enums;
using WarmHouse.Telemetry.Domain.Errors;
using WarmHouse.Telemetry.Domain.Repositories;

namespace WarmHouse.Telemetry.Application.Handlers.Commands;

/// <summary>Defines a new limit on a metric for a house the caller can reach.</summary>
public sealed class CreateThresholdRuleHandler(
    IThresholdRuleRepository rules,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<ThresholdRuleDto>> HandleAsync(
        CreateThresholdRuleRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(request.HouseId))
        {
            return AccessErrors.HouseForbidden(request.HouseId);
        }

        if (!TryParse(request.Comparison, out var comparison))
        {
            return TelemetryErrors.UnknownComparison;
        }

        var rule = ThresholdRule.Create(
            request.HouseId, request.DeviceId, request.Metric, comparison, request.Threshold, clock.UtcNow);

        rules.Add(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ThresholdRuleMapper.ToDto(rule);
    }

    private static bool TryParse(string value, out Comparison comparison)
    {
        comparison = value switch
        {
            "gt" => Comparison.GreaterThan,
            "gte" => Comparison.GreaterThanOrEqual,
            "lt" => Comparison.LessThan,
            "lte" => Comparison.LessThanOrEqual,
            _ => default,
        };

        return value is "gt" or "gte" or "lt" or "lte";
    }
}
