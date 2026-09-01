using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Domain;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Domain.DeviceTypes;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Application.UseCases.DeviceTypes;

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

        return new DeviceTypeDto(
            deviceType.Id,
            deviceType.Code,
            deviceType.Name,
            deviceType.Manufacturer,
            deviceType.Category,
            deviceType.Protocol,
            deviceType.Capabilities,
            deviceType.CreatedAt);
    }
}
