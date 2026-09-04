namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// Supplies the current time. Injected rather than calling
/// <see cref="DateTimeOffset.UtcNow"/> directly so that time-dependent domain
/// rules can be tested deterministically.
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
