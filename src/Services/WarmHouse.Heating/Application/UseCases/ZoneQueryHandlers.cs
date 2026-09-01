using WarmHouse.Heating.Application.Contracts;
using WarmHouse.Heating.Domain;
using WarmHouse.Heating.Domain.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Application.UseCases;

public sealed class ListHeatingZonesHandler(IHeatingZoneRepository zones)
{
    public async Task<Result<IReadOnlyList<HeatingZoneDto>>> HandleAsync(
        Guid? houseId,
        CancellationToken cancellationToken)
    {
        var found = await zones.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<HeatingZoneDto>>.Success(
            [.. found.Select(HeatingCommandFactory.ToDto)]);
    }
}

public sealed class GetHeatingZoneHandler(IHeatingZoneRepository zones)
{
    public async Task<Result<HeatingZoneDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var zone = await zones.GetByIdAsync(id, cancellationToken);
        return zone is null
            ? HeatingErrors.ZoneNotFound
            : Result<HeatingZoneDto>.Success(HeatingCommandFactory.ToDto(zone));
    }
}
