using WarmHouse.Gates.Application.Contracts;
using WarmHouse.Gates.Domain;
using WarmHouse.Gates.Domain.Abstractions;
using WarmHouse.Gates.Domain.Gates;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Gates.Application.UseCases;

public sealed class ListGatesHandler(IGateRepository gates)
{
    public async Task<Result<IReadOnlyList<GateDto>>> HandleAsync(
        Guid? houseId,
        CancellationToken cancellationToken)
    {
        var found = await gates.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<GateDto>>.Success([.. found.Select(GateMapper.ToDto)]);
    }
}

public sealed class GetGateHandler(IGateRepository gates)
{
    public async Task<Result<GateDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var gate = await gates.GetByIdAsync(id, cancellationToken);
        return gate is null ? GateErrors.NotFound : Result<GateDto>.Success(GateMapper.ToDto(gate));
    }
}

internal static class GateMapper
{
    public static GateDto ToDto(Gate gate) => new(
        gate.Id, gate.HouseId, gate.DeviceId, gate.Name, gate.State,
        gate.SupportsLock, gate.LastOperatedAt, gate.UpdatedAt);

    public static DeviceCommandRequested Command(
        Guid commandId, Guid deviceId, string capability, string action, Guid requestedBy, DateTimeOffset now)
        => new(Guid.CreateVersion7(), now, commandId, deviceId, capability, action,
            new Dictionary<string, string>(), requestedBy, Guid.CreateVersion7());
}
