using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Domain;

public static class MonitoringErrors
{
    public static Error CameraNotFound => Error.NotFound(
        "monitoring.camera_not_found",
        "Camera not found");

    public static Error StreamUnavailable => Error.Unavailable(
        "monitoring.stream_unavailable",
        "Stream unavailable",
        "The camera is offline or has not reported a stream address.");
}
