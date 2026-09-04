using Microsoft.Extensions.Logging;
using WarmHouse.Gates.Domain.Entities;
using WarmHouse.Gates.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Application.Handlers.Events;

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
