using WarmHouse.Shared.Kernel;

namespace WarmHouse.Shared.Application.Errors;

/// <summary>Authorisation failures every service can report.</summary>
public static class AccessErrors
{
    public static Error HouseForbidden(Guid houseId) => Error.Forbidden(
        "access.house_forbidden",
        "No access to this house",
        $"The access token does not grant access to house {houseId}. "
        + "Request a new token if access was granted after it was issued.");

    public static Error Forbidden => Error.Forbidden(
        "access.forbidden",
        "No access to this resource");
}
