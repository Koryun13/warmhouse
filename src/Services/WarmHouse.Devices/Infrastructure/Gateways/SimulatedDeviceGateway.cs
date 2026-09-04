using Microsoft.Extensions.Logging;
using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Infrastructure.Gateways;

/// <summary>
/// Stand-in for the protocol gateways that reach real hardware.
///
/// In production there would be one adapter per protocol (an MQTT broker
/// client, a Zigbee coordinator, a Modbus bridge). They all sit behind this
/// single port, so the delivery use case never learns how a given device is
/// physically reached.
/// </summary>
internal sealed class SimulatedDeviceGateway(ILogger<SimulatedDeviceGateway> logger) : IDeviceGateway
{
    public Task<bool> SendAsync(
        Device device,
        ConnectivityProtocol protocol,
        string capability,
        string action,
        IReadOnlyDictionary<string, string> payload,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Delivered {Action} ({Capability}) to device {Serial} over {Protocol}.",
            action, capability, device.SerialNumber, protocol);

        return Task.FromResult(true);
    }
}
