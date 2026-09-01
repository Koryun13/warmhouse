using WarmHouse.Gates.Application.Contracts;
using WarmHouse.Gates.Domain;
using WarmHouse.Gates.Domain.Abstractions;
using WarmHouse.Gates.Domain.Gates;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Application.UseCases;

/// <summary>The operation a caller asked for.</summary>
public enum GateOperation
{
    Open,
    Close,
    Lock,
}

/// <summary>
/// Starts a gate operation: validates it against the current state, moves the
/// gate into the matching transitional state and asks device management to
/// deliver the command.
/// </summary>
public sealed class OperateGateHandler(
    IGateRepository gates,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<GateDto>> HandleAsync(
        Guid gateId,
        GateOperation operation,
        GateOperationRequest request,
        CancellationToken cancellationToken)
    {
        var gate = await gates.GetByIdAsync(gateId, cancellationToken);
        if (gate is null)
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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            GateMapper.Command(commandId, gate.DeviceId, capability, action, request.RequestedBy, clock.UtcNow),
            cancellationToken);

        return GateMapper.ToDto(gate);
    }
}
