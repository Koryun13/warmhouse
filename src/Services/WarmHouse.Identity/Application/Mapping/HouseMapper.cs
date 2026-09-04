using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Application.Mapping;

/// <summary>Projects the house aggregate onto its published shape.</summary>
internal static class HouseMapper
{
    public static HouseDto ToDto(House house) => new(
        house.Id, house.OwnerId, house.Name, house.Address, house.TimeZone, house.CreatedAt);
}
