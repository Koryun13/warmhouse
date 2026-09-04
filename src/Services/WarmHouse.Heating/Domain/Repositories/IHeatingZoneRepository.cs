using WarmHouse.Heating.Domain.Entities;

namespace WarmHouse.Heating.Domain.Repositories;

public interface IHeatingZoneRepository
{
    Task<HeatingZone?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<HeatingZone?> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    Task<IReadOnlyList<HeatingZone>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    void Add(HeatingZone zone);

    void RemoveByDevice(Guid deviceId);
}
