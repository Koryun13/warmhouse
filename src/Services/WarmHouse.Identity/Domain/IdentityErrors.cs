using WarmHouse.Identity.Domain.Users;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Domain;

public static class IdentityErrors
{
    public static Error InvalidRegistration => Error.Validation(
        "user.invalid_registration",
        "Invalid registration data",
        $"An email and a password of at least {User.MinPasswordLength} characters are required.");

    public static Error EmailTaken => Error.Conflict("user.email_taken", "User already exists");

    public static Error UserNotFound => Error.NotFound("user.not_found", "User not found");

    /// <summary>Deliberately identical for an unknown email and a wrong password.</summary>
    public static Error InvalidCredentials => Error.Unauthorized(
        "auth.invalid_credentials",
        "Invalid credentials");

    public static Error HouseNotFound => Error.NotFound("house.not_found", "House not found");

    public static Error OwnerNotFound => Error.Unprocessable("house.owner_not_found", "Owner not found");

    public static Error AccessAlreadyGranted => Error.Conflict(
        "house.access_granted",
        "Access has already been granted");
}
