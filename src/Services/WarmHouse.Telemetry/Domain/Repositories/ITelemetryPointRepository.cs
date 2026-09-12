using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Domain.Repositories;

public interface ITelemetryPointRepository
{
    void Add(TelemetryPoint point);
}
