using Microsoft.Extensions.Logging;
using WarmHouse.Scenarios.Domain.Entities;
using WarmHouse.Scenarios.Domain.Enums;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Scenarios.Application.Handlers.Events;

/// <summary>
/// Runs the scenarios a threshold breach matches.
///
/// This service knows nothing about devices or delivery channels: it only
/// publishes events, and the specialised services carry them out. That keeps
/// automation composable — a new kind of action becomes a new subscriber, not
/// a change here.
/// </summary>
public sealed class ExecuteScenariosHandler(
    IScenarioRepository scenarios,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock,
    ILogger<ExecuteScenariosHandler> logger)
{
    public async Task HandleAsync(TelemetryThresholdBreached message, CancellationToken cancellationToken)
    {
        var matches = await scenarios.FindTelemetryTriggeredAsync(
            message.HouseId, message.DeviceId, message.Metric, cancellationToken);

        foreach (var scenario in matches)
        {
            // One correlation id per run, so the resulting commands and
            // notifications can be traced back to the scenario that caused them.
            var correlationId = Guid.CreateVersion7();

            await publisher.PublishAsync(
                new ScenarioTriggered(
                    Guid.CreateVersion7(), clock.UtcNow, scenario.Id, scenario.HouseId,
                    nameof(TriggerKind.TelemetryThreshold), correlationId),
                cancellationToken);

            foreach (var step in scenario.Steps)
            {
                await ExecuteStepAsync(step, message, correlationId, cancellationToken);
            }

            scenario.MarkTriggered(clock.UtcNow);

            logger.LogInformation(
                "Scenario {Name} ran: {Metric}={Value} breached the threshold {Threshold}.",
                scenario.Name, message.Metric, message.Value, message.Threshold);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ExecuteStepAsync(
        ScenarioStep step,
        TelemetryThresholdBreached trigger,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        if (!step.IsExecutable)
        {
            logger.LogWarning("Skipped a scenario step: required fields are missing.");
            return;
        }

        switch (step.Kind)
        {
            case ActionKind.DeviceCommand:
                await publisher.PublishAsync(
                    new DeviceCommandRequested(
                        Guid.CreateVersion7(),
                        clock.UtcNow,
                        Guid.CreateVersion7(),
                        step.DeviceId!.Value,
                        step.Capability!,
                        step.Action ?? string.Empty,
                        new Dictionary<string, string>(),
                        trigger.HouseId,
                        correlationId),
                    cancellationToken);
                break;

            case ActionKind.Notify:
                await publisher.PublishAsync(
                    new NotificationRequested(
                        Guid.CreateVersion7(),
                        clock.UtcNow,
                        step.RecipientId!.Value,
                        step.Channel ?? "push",
                        step.Subject ?? "A WarmHouse scenario has run",
                        step.Body ?? $"{trigger.Metric} = {trigger.Value} (threshold {trigger.Threshold})."),
                    cancellationToken);
                break;
        }
    }
}
