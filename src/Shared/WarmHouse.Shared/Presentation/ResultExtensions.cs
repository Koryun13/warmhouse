using Microsoft.AspNetCore.Http;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Shared.Presentation;

/// <summary>
/// Translates an application-layer <see cref="Error"/> into an HTTP response.
///
/// This is the only place that knows about status codes: handlers return
/// <see cref="Result"/> values and stay independent of the transport.
/// </summary>
public static class ResultExtensions
{
    public static IResult ToProblem(this Error error) => Results.Problem(
        title: error.Title,
        detail: error.Detail,
        statusCode: StatusCode(error.Type),
        extensions: new Dictionary<string, object?> { ["code"] = error.Code });

    /// <summary>Maps a failed result to a problem response, a successful one via <paramref name="onSuccess"/>.</summary>
    public static IResult Match<TValue>(this Result<TValue> result, Func<TValue, IResult> onSuccess)
        => result.IsSuccess ? onSuccess(result.Value) : result.Error!.ToProblem();

    public static IResult Match(this Result result, Func<IResult> onSuccess)
        => result.IsSuccess ? onSuccess() : result.Error!.ToProblem();

    private static int StatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unprocessable => StatusCodes.Status422UnprocessableEntity,
        ErrorType.Unavailable => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status500InternalServerError,
    };
}
