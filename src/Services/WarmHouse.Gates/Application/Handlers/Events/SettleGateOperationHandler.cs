using Microsoft.Extensions.Logging;
using WarmHouse.Gates.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Application.Handlers.Events;

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
