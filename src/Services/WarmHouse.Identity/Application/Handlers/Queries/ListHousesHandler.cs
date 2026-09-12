using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Queries;

/// <summary>
/// Lists the houses the caller may reach.
///
/// This service owns the membership tables, so it authorises against them
/// directly rather than against the claims in the token — a grant made after the
/// token was issued is visible here immediately.
/// </summary>
public sealed class ListHousesHandler(IHouseRepository houses, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<HouseDto>>> HandleAsync(CancellationToken cancellationToken)
    {
        var found = await houses.ListAccessibleAsync(currentUser.Id, cancellationToken);
        return Result<IReadOnlyList<HouseDto>>.Success([.. found.Select(HouseMapper.ToDto)]);
    }
}
