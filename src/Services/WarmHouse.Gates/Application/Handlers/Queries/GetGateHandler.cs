using WarmHouse.Gates.Application.Contracts.Responses;
using WarmHouse.Gates.Application.Mapping;
using WarmHouse.Gates.Domain.Errors;
using WarmHouse.Gates.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Application.Handlers.Queries;

/// <summary>Returns one gate, provided the caller can reach its house.</summary>
public sealed class GetGateHandler(IGateRepository gates, ICurrentUser currentUser)
{
    public async Task<Result<GateDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var gate = await gates.GetByIdAsync(id, cancellationToken);
        return gate is null || !currentUser.CanAccess(gate.HouseId)
            ? GateErrors.NotFound
            : Result<GateDto>.Success(GateMapper.ToDto(gate));
    }
}
