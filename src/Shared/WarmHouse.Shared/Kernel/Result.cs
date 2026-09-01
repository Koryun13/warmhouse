namespace WarmHouse.Shared.Kernel;

/// <summary>
/// Category of a failure. The presentation layer maps it to an HTTP status
/// code, so use cases never reference HTTP concepts.
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unprocessable,
    Unauthorized,
    Unavailable,
}

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

    public static Error Unavailable(string code, string title, string? detail = null)
        => new(ErrorType.Unavailable, code, title, detail);
}

/// <summary>Outcome of a use case that returns nothing.</summary>
public class Result
{
    protected Result(Error? error) => Error = error;

    public Error? Error { get; }

    public bool IsSuccess => Error is null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);
}

/// <summary>Outcome of a use case that returns a value.</summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue? value, Error? error) : base(error) => _value = value;

    /// <summary>Only valid when <see cref="Result.IsSuccess"/> is true.</summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read the value of a failed result.");

    public static Result<TValue> Success(TValue value) => new(value, null);

    public static new Result<TValue> Failure(Error error) => new(default, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure(error);
}
