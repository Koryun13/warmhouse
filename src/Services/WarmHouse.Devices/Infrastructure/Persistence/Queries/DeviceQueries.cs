using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Application.Mapping;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Infrastructure.Persistence.Queries;

/// <summary>
/// Read side of the service.
///
/// Device and device type are joined in one query. This is deliberate: the
/// As-Is monolith issued a separate call per device when listing them, and that
/// N+1 was the main reason its list endpoint degraded as homes were added.
/// </summary>
internal sealed class DeviceQueries(DevicesDbContext context) : IDeviceQueries
{
    public async Task<IReadOnlyList<DeviceTypeDto>> ListDeviceTypesAsync(
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        var query = context.DeviceTypes.AsNoTracking();

        if (category is { } value)
        {
            query = query.Where(t => t.Category == value);
        }

        var types = await query.OrderBy(t => t.Code).ToListAsync(cancellationToken);

        return [.. types.Select(DeviceTypeMapper.ToDto)];
    }

    public async Task<IReadOnlyList<DeviceDto>> ListDevicesAsync(
        Guid? houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        // An anonymous projection keeps the join translatable to SQL; a custom
        // type holding two entities is not something EF Core can translate.
        var query = context.Devices
            .AsNoTracking()
            .Join(
                context.DeviceTypes.AsNoTracking(),
                device => device.DeviceTypeId,
                type => type.Id,
                (device, type) => new { Device = device, Type = type });

        if (houseId is { } house)
        {
            query = query.Where(row => row.Device.HouseId == house);
        }

        if (category is { } value)
        {
            query = query.Where(row => row.Type.Category == value);
        }

        var rows = await query
            .OrderBy(row => row.Device.RegisteredAt)
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => DeviceMapper.ToDto(row.Device, row.Type))];
    }

    public async Task<DeviceDto?> GetDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var row = await context.Devices
            .AsNoTracking()
            .Where(device => device.Id == deviceId)
            .Join(
                context.DeviceTypes.AsNoTracking(),
                device => device.DeviceTypeId,
                type => type.Id,
                (device, type) => new { Device = device, Type = type })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : DeviceMapper.ToDto(row.Device, row.Type);
    }

    public async Task<DeviceCommandDto?> GetCommandAsync(
        Guid deviceId,
        Guid commandId,
        CancellationToken cancellationToken)
    {
        var command = await context.Commands
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commandId && c.DeviceId == deviceId, cancellationToken);

        return command is null ? null : DeviceCommandMapper.ToDto(command);
    }
}
