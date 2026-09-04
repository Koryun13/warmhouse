using WarmHouse.Scenarios.Application.Contracts.Requests;
using WarmHouse.Scenarios.Application.Contracts.Responses;
using WarmHouse.Scenarios.Application.Mapping;
using WarmHouse.Scenarios.Domain.Entities;
using WarmHouse.Scenarios.Domain.Enums;
using WarmHouse.Scenarios.Domain.Errors;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.Handlers.Commands;

/// <summary>Composes a new automation rule for a house the caller can reach.</summary>
public sealed class CreateScenarioHandler(
    IScenarioRepository scenarios,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<ScenarioDto>> HandleAsync(
        CreateScenarioRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(request.HouseId))
        {
            return AccessErrors.HouseForbidden(request.HouseId);
        }

        if (request.Actions.Count == 0)
        {
            return ScenarioErrors.NoSteps;
        }

        if (request.TriggerKind == TriggerKind.TelemetryThreshold
            && string.IsNullOrWhiteSpace(request.TriggerMetric))
        {
            return ScenarioErrors.MetricRequired;
        }

        var scenario = Scenario.Create(
            request.HouseId,
            request.Name,
            request.TriggerKind,
            request.TriggerMetric,
            request.TriggerDeviceId,
            request.Actions.Select(ScenarioMapper.ToStep),
            clock.UtcNow);

        scenarios.Add(scenario);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ScenarioMapper.ToDto(scenario);
    }
}
