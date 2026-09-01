using WarmHouse.Scenarios.Application.Contracts;
using WarmHouse.Scenarios.Domain;
using WarmHouse.Scenarios.Domain.Abstractions;
using WarmHouse.Scenarios.Domain.Automation;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Application.UseCases;

public sealed class ListScenariosHandler(IScenarioRepository scenarios)
{
    public async Task<Result<IReadOnlyList<ScenarioDto>>> HandleAsync(
        Guid? houseId,
        CancellationToken cancellationToken)
    {
        var found = await scenarios.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<ScenarioDto>>.Success([.. found.Select(ScenarioMapper.ToDto)]);
    }
}

public sealed class GetScenarioHandler(IScenarioRepository scenarios)
{
    public async Task<Result<ScenarioDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        return scenario is null
            ? ScenarioErrors.NotFound
            : Result<ScenarioDto>.Success(ScenarioMapper.ToDto(scenario));
    }
}

public sealed class CreateScenarioHandler(
    IScenarioRepository scenarios,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<ScenarioDto>> HandleAsync(
        CreateScenarioRequest request,
        CancellationToken cancellationToken)
    {
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

public sealed class SetScenarioEnabledHandler(IScenarioRepository scenarios, IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, bool enabled, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        if (scenario is null)
        {
            return Result.Failure(ScenarioErrors.NotFound);
        }

        scenario.SetEnabled(enabled);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class DeleteScenarioHandler(IScenarioRepository scenarios, IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var scenario = await scenarios.GetByIdAsync(id, cancellationToken);
        if (scenario is null)
        {
            return Result.Failure(ScenarioErrors.NotFound);
        }

        scenarios.Remove(scenario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

internal static class ScenarioMapper
{
    public static ScenarioStep ToStep(ScenarioStepDto dto) => new()
    {
        Kind = dto.Kind,
        DeviceId = dto.DeviceId,
        Capability = dto.Capability,
        Action = dto.Action,
        RecipientId = dto.RecipientId,
        Channel = dto.Channel,
        Subject = dto.Subject,
        Body = dto.Body,
    };

    public static ScenarioDto ToDto(Scenario scenario) => new(
        scenario.Id,
        scenario.HouseId,
        scenario.Name,
        scenario.Enabled,
        scenario.Trigger,
        scenario.TriggerMetric,
        scenario.TriggerDeviceId,
        [.. scenario.Steps.Select(s => new ScenarioStepDto(
            s.Kind, s.DeviceId, s.Capability, s.Action, s.RecipientId, s.Channel, s.Subject, s.Body))],
        scenario.CreatedAt,
        scenario.LastTriggeredAt);
}
