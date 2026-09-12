using WarmHouse.Lighting.Application.Contracts.Responses;
using WarmHouse.Lighting.Domain.Entities;

namespace WarmHouse.Lighting.Application.Mapping;

/// <summary>Projects the fixture aggregate onto its published shape.</summary>
public static class LightFixtureMapper
{
    public static LightFixtureDto ToDto(LightFixture fixture) => new(
        fixture.Id, fixture.HouseId, fixture.DeviceId, fixture.Name, fixture.Location,
        fixture.IsOn, fixture.Brightness, fixture.IsDimmable, fixture.UpdatedAt);
}
