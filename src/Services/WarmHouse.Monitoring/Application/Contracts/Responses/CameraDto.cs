namespace WarmHouse.Monitoring.Application.Contracts.Responses;

/// <summary>A camera as the API publishes it; the stream address stays internal.</summary>
public sealed record CameraDto(
    Guid Id,
    Guid HouseId,
    Guid DeviceId,
    string Name,
    string? Location,
    bool IsOnline,
    bool IsRecording,
    DateTimeOffset? LastSnapshotAt,
    DateTimeOffset UpdatedAt);
