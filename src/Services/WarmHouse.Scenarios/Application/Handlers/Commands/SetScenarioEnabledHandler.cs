using WarmHouse.Scenarios.Domain.Errors;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.Handlers.Commands;

/// <summary>Turns an automation rule on or off without deleting it.</summary>
public sealed class SetScenarioEnabledHandler(
    IScenarioRepository scenarios,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, bool enabled, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        if (scenario is null || !currentUser.CanAccess(scenario.HouseId))
        {
            return Result.Failure(ScenarioErrors.NotFound);
        }

        scenario.SetEnabled(enabled);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
