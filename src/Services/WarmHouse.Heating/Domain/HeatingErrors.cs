using WarmHouse.Heating.Domain.Zones;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Domain;

public static class HeatingErrors
{
    public static Error ZoneNotFound => Error.NotFound(
        "heating.zone_not_found",
        "Heating zone not found");

    public static Error TemperatureOutOfRange => Error.Unprocessable(
        "heating.temperature_out_of_range",
        "Temperature out of range",
        $"The target must be between {HeatingZone.MinTemperature} and {HeatingZone.MaxTemperature} degrees Celsius.");
}
