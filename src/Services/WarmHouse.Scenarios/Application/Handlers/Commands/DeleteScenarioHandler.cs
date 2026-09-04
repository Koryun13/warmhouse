using WarmHouse.Scenarios.Domain.Errors;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.Handlers.Commands;

/// <summary>Removes an automation rule from a house the caller can reach.</summary>
public sealed class DeleteScenarioHandler(
    IScenarioRepository scenarios,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        if (scenario is null || !currentUser.CanAccess(scenario.HouseId))
        {
            return Result.Failure(ScenarioErrors.NotFound);
        }

        scenarios.Remove(scenario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
