using WarmHouse.Devices.Application.Contracts.Requests;
using WarmHouse.Devices.Application.Contracts.Responses;
using WarmHouse.Devices.Domain.Entities;
using WarmHouse.Devices.Domain.Errors;
using WarmHouse.Devices.Application.Mapping;
using WarmHouse.Devices.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.Handlers.Commands;

/// <summary>
/// Registers a previously unknown class of device.
///
/// This single use case is how the ecosystem absorbs hardware nobody had
/// anticipated: a partner declares the capabilities of their product and it
/// becomes connectable, with no code change anywhere in the system.
/// </summary>
public sealed class CreateDeviceTypeHandler(
    IDeviceTypeRepository repository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<DeviceTypeDto>> HandleAsync(
        CreateDeviceTypeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
        {
            return DeviceErrors.InvalidDeviceType;
        }

        if (await repository.ExistsAsync(request.Code, cancellationToken))
        {
            return DeviceErrors.DeviceTypeCodeTaken(request.Code);
        }

        var deviceType = DeviceType.Create(
            request.Code,
            request.Name,
            request.Manufacturer,
            request.Category,
            request.Protocol,
            request.Capabilities,
            clock.UtcNow);

        repository.Add(deviceType);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return DeviceTypeMapper.ToDto(deviceType);
    }
}
