namespace WarmHouse.Monitoring.Application.Contracts;

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

/// <summary>Short-lived permission to open a stream, instead of the raw address.</summary>
public sealed record StreamTicketDto(Guid CameraId, string StreamUrl, DateTimeOffset ExpiresAt);

// The requester is the authenticated caller, taken from the token.
public sealed record SetRecordingRequest(bool Enabled);
