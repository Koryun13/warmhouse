using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Domain.Errors;
using WarmHouse.Telemetry.Domain.Repositories;

namespace WarmHouse.Telemetry.Application.Handlers.Commands;

/// <summary>Removes a threshold rule from a house the caller can reach.</summary>
public sealed class DeleteThresholdRuleHandler(
    IThresholdRuleRepository rules,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var rule = await rules.GetByIdAsync(id, cancellationToken);
        if (rule is null || !currentUser.CanAccess(rule.HouseId))
        {
            return Result.Failure(TelemetryErrors.RuleNotFound);
        }

        rules.Remove(rule);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
