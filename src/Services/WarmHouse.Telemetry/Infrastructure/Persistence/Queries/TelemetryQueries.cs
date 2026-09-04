using Microsoft.EntityFrameworkCore;
using WarmHouse.Telemetry.Application.Abstractions;
using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Application.Contracts.Responses;

namespace WarmHouse.Telemetry.Infrastructure.Persistence.Queries;

internal sealed class TelemetryQueries(TelemetryDbContext context) : ITelemetryQueries
{
    private const int DefaultLimit = 100;
    private const int MaxLimit = 1000;

    public async Task<IReadOnlyList<MeasurementDto>> QueryAsync(
        TelemetryQuery query,
        CancellationToken cancellationToken)
    {
        var points = context.Points
            .AsNoTracking()
            .Where(p => p.HouseId == query.HouseId && p.DeviceId == query.DeviceId);

        if (!string.IsNullOrWhiteSpace(query.Metric))
        {
            var metric = query.Metric.Trim().ToLowerInvariant();
            points = points.Where(p => p.Metric == metric);
        }

        if (query.From is { } from)
        {
            points = points.Where(p => p.MeasuredAt >= from);
        }

        if (query.To is { } to)
        {
            points = points.Where(p => p.MeasuredAt <= to);
        }

        // The limit is always applied: an unbounded history query would be a
        // denial-of-service vector against the time-series table.
        return await points
            .OrderByDescending(p => p.MeasuredAt)
            .Take(Math.Clamp(query.Limit ?? DefaultLimit, 1, MaxLimit))
            .Select(p => new MeasurementDto(p.DeviceId, p.Metric, p.Value, p.Unit, p.MeasuredAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<MeasurementDto?> GetLatestAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken)
    {
        var normalized = metric.Trim().ToLowerInvariant();

        return await context.Points
            .AsNoTracking()
            .Where(p => p.HouseId == houseId && p.DeviceId == deviceId && p.Metric == normalized)
            .OrderByDescending(p => p.MeasuredAt)
            .Select(p => new MeasurementDto(p.DeviceId, p.Metric, p.Value, p.Unit, p.MeasuredAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
