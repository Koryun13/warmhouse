using WarmHouse.Shared.Kernel;

namespace WarmHouse.Scenarios.Domain.Errors;

public static class ScenarioErrors
{
    public static Error NotFound => Error.NotFound("scenario.not_found", "Scenario not found");

    public static Error NoSteps => Error.Validation(
        "scenario.no_steps",
        "Empty scenario",
        "A scenario must contain at least one action.");

    public static Error MetricRequired => Error.Validation(
        "scenario.metric_required",
        "Trigger metric is missing",
        "A telemetry trigger requires the 'trigger_metric' field.");
}
