using Microsoft.EntityFrameworkCore;
using WarmHouse.Telemetry.Domain.Entities;
using WarmHouse.Telemetry.Domain.Repositories;

namespace WarmHouse.Telemetry.Infrastructure.Persistence.Repositories;

internal sealed class TelemetryPointRepository(TelemetryDbContext context) : ITelemetryPointRepository
{
    public void Add(TelemetryPoint point) => context.Points.Add(point);
}
