using WarmHouse.Shared.Application.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Hosting;

/// <summary>Wall-clock implementation of the application's time port.</summary>
internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
