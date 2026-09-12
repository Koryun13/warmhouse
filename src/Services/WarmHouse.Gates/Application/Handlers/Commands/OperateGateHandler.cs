using WarmHouse.Gates.Application.Contracts.Requests;
using WarmHouse.Gates.Application.Contracts.Responses;
using WarmHouse.Gates.Application.Mapping;
using WarmHouse.Gates.Domain.Enums;
using WarmHouse.Gates.Domain.Errors;
using WarmHouse.Gates.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Application.Handlers.Commands;

/// <summary>
/// Starts a gate operation: validates it against the current state, moves the
/// gate into the matching transitional state and asks device management to
/// deliver the command.
/// </summary>
public sealed class OperateGateHandler(
    IGateRepository gates,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<GateDto>> HandleAsync(
        Guid gateId,
        GateOperation operation,
        CancellationToken cancellationToken)
    {
        var gate = await gates.GetByIdAsync(gateId, cancellationToken);
        if (gate is null || !currentUser.CanAccess(gate.HouseId))
        {
            return GateErrors.NotFound;
        }

        if (operation is GateOperation.Open or GateOperation.Close && gate.IsLocked)
        {
            return GateErrors.Locked;
        }

        if (operation == GateOperation.Lock)
        {
            if (!gate.SupportsLock)
            {
                return GateErrors.LockNotSupported;
            }

            if (!gate.CanLock)
            {
                return GateErrors.NotClosed(gate.State.ToString());
            }
        }

        var (capability, action, transitionalState) = operation switch
        {
            GateOperation.Open => ("gate.open", "open", GateState.Opening),
            GateOperation.Close => ("gate.close", "close", GateState.Closing),
            _ => ("gate.lock", "lock", GateState.Locked),
        };

        var commandId = Guid.CreateVersion7();
        gate.BeginOperation(commandId, transitionalState, clock.UtcNow);

        // Outbox: the gate cannot be left waiting for a command that was never
        // sent, nor a command sent for a transition that was never recorded.
        await publisher.PublishAsync(
            GateMapper.Command(commandId, gate.DeviceId, capability, action, currentUser.Id, clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GateMapper.ToDto(gate);
    }
}
