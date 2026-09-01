using WarmHouse.Lighting.Domain.Fixtures;

namespace WarmHouse.Lighting.Domain.Abstractions;

public interface ILightFixtureRepository
{
    Task<LightFixture?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<LightFixture>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken);

    void Add(LightFixture fixture);

    void RemoveByDevice(Guid deviceId);
}
