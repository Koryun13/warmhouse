using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Domain.Entities;

namespace WarmHouse.Monitoring.Application.Mapping;

/// <summary>
/// Builds the ticket handed to a caller instead of the camera address. Only a
/// camera that passed <see cref="Camera.CanStream"/> reaches this point, which
/// is what makes the stream address non-null here.
/// </summary>
public static class StreamTicketMapper
{
    public static StreamTicketDto ToDto(Camera camera, DateTimeOffset expiresAt) => new(
        camera.Id,
        camera.StreamUrl!,
        expiresAt);
}
