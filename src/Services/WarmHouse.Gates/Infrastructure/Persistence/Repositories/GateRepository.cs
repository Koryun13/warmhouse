using Microsoft.EntityFrameworkCore;
using WarmHouse.Gates.Domain.Abstractions;
using WarmHouse.Gates.Domain.Gates;

namespace WarmHouse.Gates.Infrastructure.Persistence.Repositories;

internal sealed class GateRepository(GatesDbContext context) : IGateRepository
{
    public Task<Gate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Gates.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public Task<Gate?> GetByPendingCommandAsync(Guid commandId, CancellationToken cancellationToken)
        => context.Gates.FirstOrDefaultAsync(g => g.PendingCommandId == commandId, cancellationToken);

    public async Task<IReadOnlyList<Gate>> ListAsync(Guid? houseId, CancellationToken cancellationToken)
    {
        var query = context.Gates.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(g => g.HouseId == house);
        }

        return await query.OrderBy(g => g.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
        => context.Gates.AnyAsync(g => g.DeviceId == deviceId, cancellationToken);

    public void Add(Gate gate) => context.Gates.Add(gate);

    public void RemoveByDevice(Guid deviceId)
        => context.Gates.RemoveRange(context.Gates.Where(g => g.DeviceId == deviceId));
}
