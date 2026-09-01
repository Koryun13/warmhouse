using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain.DeviceTypes;
using WarmHouse.Shared.Contracts.Enums;
using DeviceEntity = WarmHouse.Devices.Domain.Devices.Device;

namespace WarmHouse.Devices.Infrastructure.Persistence;

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

        return [.. types.Select(Map)];
    }

    public async Task<IReadOnlyList<DeviceDto>> ListDevicesAsync(
        Guid? houseId,
        DeviceCategory? category,
        CancellationToken cancellationToken)
    {
        // An anonymous projection keeps the join translatable to SQL; a custom
        // type holding two entities is not something EF Core can translate.
        var query = from device in context.Devices.AsNoTracking()
                    join type in context.DeviceTypes.AsNoTracking()
                        on device.DeviceTypeId equals type.Id
                    select new { Device = device, Type = type };

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

        return [.. rows.Select(row => Map(row.Device, row.Type))];
    }

    public async Task<DeviceDto?> GetDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        var row = await (from device in context.Devices.AsNoTracking()
                         join type in context.DeviceTypes.AsNoTracking()
                             on device.DeviceTypeId equals type.Id
                         where device.Id == deviceId
                         select new { Device = device, Type = type })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : Map(row.Device, row.Type);
    }

    public async Task<DeviceCommandDto?> GetCommandAsync(
        Guid deviceId,
        Guid commandId,
        CancellationToken cancellationToken)
    {
        var command = await context.Commands
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commandId && c.DeviceId == deviceId, cancellationToken);

        return command is null
            ? null
            : new DeviceCommandDto(
                command.Id,
                command.DeviceId,
                command.Capability,
                command.Action,
                command.Payload,
                command.Status,
                command.Error,
                command.CorrelationId,
                command.RequestedAt,
                command.CompletedAt);
    }

    private static DeviceTypeDto Map(DeviceType type) => new(
        type.Id,
        type.Code,
        type.Name,
        type.Manufacturer,
        type.Category,
        type.Protocol,
        type.Capabilities,
        type.CreatedAt);

    private static DeviceDto Map(DeviceEntity device, DeviceType type) => new(
        device.Id,
        device.HouseId,
        device.OwnerId,
        device.DeviceTypeId,
        type.Code,
        type.Category,
        device.SerialNumber,
        device.Name,
        device.Location,
        device.Status,
        device.Firmware,
        type.Capabilities,
        device.LastSeenAt,
        device.RegisteredAt);
}
