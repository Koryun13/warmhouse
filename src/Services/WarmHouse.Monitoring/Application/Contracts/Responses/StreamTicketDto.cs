namespace WarmHouse.Monitoring.Application.Contracts.Responses;

/// <summary>Short-lived permission to open a stream, instead of the raw address.</summary>
public sealed record StreamTicketDto(Guid CameraId, string StreamUrl, DateTimeOffset ExpiresAt);
