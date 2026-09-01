using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Contracts;
using WarmHouse.Telemetry.Domain;
using WarmHouse.Telemetry.Domain.Abstractions;
using WarmHouse.Telemetry.Domain.Thresholds;

namespace WarmHouse.Telemetry.Application.UseCases.Thresholds;

public sealed class ListThresholdRulesHandler(IThresholdRuleRepository rules)
{
    public async Task<Result<IReadOnlyList<ThresholdRuleDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        var found = await rules.ListForHouseAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<ThresholdRuleDto>>.Success([.. found.Select(Map)]);
    }

    internal static ThresholdRuleDto Map(ThresholdRule rule) => new(
        rule.Id, rule.HouseId, rule.DeviceId, rule.Metric,
        rule.OperatorCode, rule.Threshold, rule.Enabled, rule.CreatedAt);
}

public sealed class CreateThresholdRuleHandler(
    IThresholdRuleRepository rules,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<ThresholdRuleDto>> HandleAsync(
        CreateThresholdRuleRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryParse(request.Comparison, out var comparison))
        {
            return TelemetryErrors.UnknownComparison;
        }

        var rule = ThresholdRule.Create(
            request.HouseId, request.DeviceId, request.Metric, comparison, request.Threshold, clock.UtcNow);

        rules.Add(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ListThresholdRulesHandler.Map(rule);
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

public sealed class DeleteThresholdRuleHandler(IThresholdRuleRepository rules, IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(id, cancellationToken);
        if (rule is null)
        {
            return Result.Failure(TelemetryErrors.RuleNotFound);
        }

        rules.Remove(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
