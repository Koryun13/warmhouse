using WarmHouse.Gates.Application.Contracts.Responses;
using WarmHouse.Gates.Application.Mapping;
using WarmHouse.Gates.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Application.Handlers.Queries;

/// <summary>Lists the gates of one house the caller has access to.</summary>
public sealed class ListGatesHandler(IGateRepository gates, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<GateDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await gates.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<GateDto>>.Success([.. found.Select(GateMapper.ToDto)]);
    }
}
