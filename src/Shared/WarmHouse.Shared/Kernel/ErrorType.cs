namespace WarmHouse.Shared.Kernel;

/// <summary>
/// Category of a failure. The presentation layer maps it to an HTTP status
/// code, so handlers never reference HTTP concepts.
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unprocessable,
    Unauthorized,
    Forbidden,
    Unavailable,
}
