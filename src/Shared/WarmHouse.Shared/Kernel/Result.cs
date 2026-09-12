namespace WarmHouse.Shared.Kernel;

/// <summary>Outcome of a handler that returns nothing.</summary>
public class Result
{
    protected Result(Error? error) => Error = error;

    public Error? Error { get; }

    public bool IsSuccess => Error is null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);
}
