using WarmHouse.Scenarios.Application.Contracts.Responses;
using WarmHouse.Scenarios.Application.Mapping;
using WarmHouse.Scenarios.Domain.Errors;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.Handlers.Queries;

/// <summary>Returns one automation rule, provided the caller can reach its house.</summary>
public sealed class GetScenarioHandler(IScenarioRepository scenarios, ICurrentUser currentUser)
{
    public async Task<Result<ScenarioDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        return scenario is null || !currentUser.CanAccess(scenario.HouseId)
            ? ScenarioErrors.NotFound
            : Result<ScenarioDto>.Success(ScenarioMapper.ToDto(scenario));
    }
}
