using WarmHouse.Gates.Domain.Entities;

namespace WarmHouse.Gates.Domain.Repositories;

public interface IGateRepository
{
    Task<Gate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Gate?> GetByPendingCommandAsync(Guid commandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Gate>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    void Add(Gate gate);

    void RemoveByDevice(Guid deviceId);
}
