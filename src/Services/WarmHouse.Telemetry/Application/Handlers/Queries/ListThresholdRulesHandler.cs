using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Contracts.Responses;
using WarmHouse.Telemetry.Application.Mapping;
using WarmHouse.Telemetry.Domain.Repositories;

namespace WarmHouse.Telemetry.Application.Handlers.Queries;

/// <summary>Lists the threshold rules of one house the caller has access to.</summary>
public sealed class ListThresholdRulesHandler(IThresholdRuleRepository rules, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<ThresholdRuleDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await rules.ListForHouseAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<ThresholdRuleDto>>.Success([.. found.Select(ThresholdRuleMapper.ToDto)]);
    }
}
