using WarmHouse.Monitoring.Domain.Cameras;

namespace WarmHouse.Monitoring.Domain.Abstractions;

public interface ICameraRepository
{
    Task<Camera?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Camera?> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Camera>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    void Add(Camera camera);

    void RemoveByDevice(Guid deviceId);
}
