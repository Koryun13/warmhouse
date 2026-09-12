using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Domain.Entities;

/// <summary>
/// A surveillance camera, projected from a device of the camera category.
///
/// The stream address is held here but never handed out directly: callers get
/// a short-lived ticket instead, so the camera is not addressable from outside
/// the platform.
/// </summary>
public sealed class Camera : AggregateRoot
{
    /// <summary>How long a stream ticket stays valid.</summary>
    public static readonly TimeSpan TicketLifetime = TimeSpan.FromMinutes(5);

    private Camera()
    {
        // Required by EF Core.
    }

    private Camera(Guid id, Guid houseId, Guid deviceId, string name, string? streamUrl, DateTimeOffset now)
        : base(id)
    {
        HouseId = houseId;
        DeviceId = deviceId;
        Name = name;
        StreamUrl = streamUrl;
        IsOnline = true;
        UpdatedAt = now;
    }

    public Guid HouseId { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Location { get; private set; }

    public string? StreamUrl { get; private set; }

    public bool IsRecording { get; private set; }

    public bool IsOnline { get; private set; }

    public DateTimeOffset? LastSnapshotAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Camera ForDevice(
        Guid houseId, Guid deviceId, string name, string? streamUrl, DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, deviceId, name, streamUrl, now);

    public bool CanStream => IsOnline && !string.IsNullOrWhiteSpace(StreamUrl);

    public void SetRecording(bool enabled, DateTimeOffset now)
    {
        IsRecording = enabled;
        UpdatedAt = now;
    }

    public void SetOnline(bool online, DateTimeOffset now)
    {
        IsOnline = online;
        UpdatedAt = now;
    }
}
