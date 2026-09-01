using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Domain;

public static class LightingErrors
{
    public static Error FixtureNotFound => Error.NotFound(
        "lighting.fixture_not_found",
        "Light fixture not found");

    public static Error NotDimmable => Error.Unprocessable(
        "lighting.not_dimmable",
        "Brightness is not supported",
        "This fixture does not declare the lighting.brightness capability.");

    public static Error BrightnessOutOfRange => Error.Unprocessable(
        "lighting.brightness_out_of_range",
        "Brightness out of range",
        "The value must be between 0 and 100.");
}
