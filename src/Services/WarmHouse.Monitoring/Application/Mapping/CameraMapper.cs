using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Domain.Entities;

namespace WarmHouse.Monitoring.Application.Mapping;

/// <summary>Projects the camera aggregate onto its published shape.</summary>
internal static class CameraMapper
{
    public static CameraDto ToDto(Camera camera) => new(
        camera.Id, camera.HouseId, camera.DeviceId, camera.Name, camera.Location,
        camera.IsOnline, camera.IsRecording, camera.LastSnapshotAt, camera.UpdatedAt);
}
