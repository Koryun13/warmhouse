using WarmHouse.Shared.Kernel;

namespace WarmHouse.Telemetry.Domain.Errors;

public static class TelemetryErrors
{
    public static Error MetricRequired => Error.Validation(
        "telemetry.metric_required",
        "Invalid measurement",
        "Field 'metric' is required.");

    public static Error UnknownComparison => Error.Validation(
        "threshold.invalid_comparison",
        "Invalid comparison operator",
        "Allowed values for 'comparison' are: gt, gte, lt, lte.");

    public static Error NoMeasurements => Error.NotFound(
        "telemetry.not_found",
        "No measurements found");

    public static Error RuleNotFound => Error.NotFound(
        "threshold.not_found",
        "Threshold rule not found");
}
