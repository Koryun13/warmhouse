using WarmHouse.Scenarios.Application.Contracts.Responses;
using WarmHouse.Scenarios.Application.Mapping;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.Handlers.Queries;

/// <summary>Lists the automation rules of one house the caller has access to.</summary>
public sealed class ListScenariosHandler(IScenarioRepository scenarios, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<ScenarioDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await scenarios.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<ScenarioDto>>.Success([.. found.Select(ScenarioMapper.ToDto)]);
    }
}
