using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Errors;

/// <summary>
/// The failures this bounded context can report. Declaring them in one place
/// keeps error codes stable across use cases and out of the endpoints.
/// </summary>
public static class DeviceErrors
{
    public static Error DeviceTypeNotFound(string code) => Error.Unprocessable(
        "device_type.not_found",
        "Unknown device type",
        $"Device type '{code}' is not registered in the catalogue.");

    public static Error DeviceTypeCodeTaken(string code) => Error.Conflict(
        "device_type.code_taken",
        "Device type already registered",
        $"Code '{code}' is already in use.");

    public static Error InvalidDeviceType => Error.Validation(
        "device_type.invalid",
        "Invalid device type",
        "Fields 'code' and 'name' are required.");

    public static Error DeviceNotFound => Error.NotFound(
        "device.not_found",
        "Device not found");

    public static Error SerialNumberTaken(string serialNumber) => Error.Conflict(
        "device.serial_taken",
        "Device already connected",
        $"Serial number '{serialNumber}' is already in use.");

    public static Error CapabilityNotSupported(string capability, IEnumerable<string> supported) => Error.Unprocessable(
        "device.capability_unsupported",
        "Capability not supported",
        $"The device does not support '{capability}'. Supported: {string.Join(", ", supported)}.");

    public static Error CommandNotFound => Error.NotFound(
        "command.not_found",
        "Command not found");
}
