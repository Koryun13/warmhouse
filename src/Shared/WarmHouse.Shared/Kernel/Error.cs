namespace WarmHouse.Shared.Kernel;

/// <summary>A failure with a stable code, a human-readable title and details.</summary>
public sealed record Error(ErrorType Type, string Code, string Title, string? Detail = null)
{
    public static Error Validation(string code, string title, string? detail = null)
        => new(ErrorType.Validation, code, title, detail);

    public static Error NotFound(string code, string title, string? detail = null)
        => new(ErrorType.NotFound, code, title, detail);

    public static Error Conflict(string code, string title, string? detail = null)
        => new(ErrorType.Conflict, code, title, detail);

    public static Error Unprocessable(string code, string title, string? detail = null)
        => new(ErrorType.Unprocessable, code, title, detail);

    public static Error Unauthorized(string code, string title, string? detail = null)
        => new(ErrorType.Unauthorized, code, title, detail);

    /// <summary>The caller is known but must not touch this resource.</summary>
    public static Error Forbidden(string code, string title, string? detail = null)
        => new(ErrorType.Forbidden, code, title, detail);

    public static Error Unavailable(string code, string title, string? detail = null)
        => new(ErrorType.Unavailable, code, title, detail);
}
