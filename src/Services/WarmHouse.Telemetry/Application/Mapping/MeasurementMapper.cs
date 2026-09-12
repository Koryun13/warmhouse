using System.Linq.Expressions;
using WarmHouse.Telemetry.Application.Contracts.Responses;
using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Application.Mapping;

/// <summary>Projects a stored reading onto its published shape.</summary>
public static class MeasurementMapper
{
    /// <summary>
    /// Kept as an expression rather than a method so EF Core translates it into
    /// the SELECT list: the time-series table is read column-wise, without
    /// materialising a point per row.
    /// </summary>
    public static Expression<Func<TelemetryPoint, MeasurementDto>> Projection { get; } =
        point => new MeasurementDto(
            point.DeviceId,
            point.Metric,
            point.Value,
            point.Unit,
            point.MeasuredAt);
}
