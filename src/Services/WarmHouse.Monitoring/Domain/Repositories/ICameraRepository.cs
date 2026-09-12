using WarmHouse.Monitoring.Domain.Entities;

namespace WarmHouse.Monitoring.Domain.Repositories;

public interface ICameraRepository
{
    Task<Camera?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Camera?> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Camera>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    void Add(Camera camera);

    void RemoveByDevice(Guid deviceId);
}
