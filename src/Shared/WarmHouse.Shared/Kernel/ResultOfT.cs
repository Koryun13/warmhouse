namespace WarmHouse.Shared.Kernel;

/// <summary>Outcome of a handler that returns a value.</summary>
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
