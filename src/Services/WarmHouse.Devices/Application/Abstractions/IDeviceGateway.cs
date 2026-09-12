using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Abstractions;

/// <summary>
/// Port for the protocol gateway that physically reaches a device. The
/// implementation lives in the infrastructure layer; the MVP simulates it.
/// </summary>
public interface IDeviceGateway
{
    Task<bool> SendAsync(
        Device device,
        ConnectivityProtocol protocol,
        string capability,
        string action,
        IReadOnlyDictionary<string, string> payload,
        CancellationToken cancellationToken);
}
