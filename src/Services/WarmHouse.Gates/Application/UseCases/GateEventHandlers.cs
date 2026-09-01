using Microsoft.Extensions.Logging;
using WarmHouse.Gates.Domain.Abstractions;
using WarmHouse.Gates.Domain.Gates;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Application.UseCases;

/// <summary>Projects gate devices into this service.</summary>
public sealed class ProjectGateDeviceHandler(
    IGateRepository gates,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    ILogger<ProjectGateDeviceHandler> logger)
{
    public async Task HandleAsync(DeviceRegistered message, CancellationToken cancellationToken)
    {
        var isGate = message.Category == DeviceCategory.Gate
                     || message.Capabilities.Contains("gate.open");

        if (!isGate || await gates.ExistsForDeviceAsync(message.DeviceId, cancellationToken))
        {
            return;
        }

        gates.Add(Gate.ForDevice(
            message.HouseId,
            message.DeviceId,
            $"Gate {message.SerialNumber}",
            message.Capabilities.Contains("gate.lock"),
            clock.UtcNow));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Registered a gate for device {DeviceId}.", message.DeviceId);
    }

    public async Task HandleAsync(DeviceDecommissioned message, CancellationToken cancellationToken)
    {
        gates.RemoveByDevice(message.DeviceId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Completes the open/close/lock saga when the drive reports the outcome.
/// Until this arrives the gate stays in its transitional state, which is what
/// the user sees in the app.
/// </summary>
public sealed class SettleGateOperationHandler(
    IGateRepository gates,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    ILogger<SettleGateOperationHandler> logger)
{
    public async Task HandleAsync(DeviceCommandCompleted message, CancellationToken cancellationToken)
    {
        var gate = await gates.GetByPendingCommandAsync(message.CommandId, cancellationToken);
        if (gate is null)
        {
            return;
        }

        gate.SettleOperation(
            message.CommandId, message.Status == CommandStatus.Acknowledged, clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Gate {GateId} settled in state {State}.", gate.Id, gate.State);
    }
}
