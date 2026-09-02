using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// Outbound port for publishing integration events.
///
/// Use cases depend on this interface, never on MassTransit or RabbitMQ, which
/// keeps the application layer free of transport concerns and testable with a
/// simple fake.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent;
}

/// <summary>
/// Commits the changes made to aggregates within a single use case.
/// Implemented by the persistence layer over the service's DbContext.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Supplies the current time. Injected rather than calling
/// <see cref="DateTimeOffset.UtcNow"/> directly so that time-dependent domain
/// rules can be tested deterministically.
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

/// <summary>
/// The authenticated caller of the current request.
///
/// Use cases take the identity from here and never from the request body: a
/// client that could name its own user id could act as anybody. The houses come
/// from the access token, so a service authorises a request without a synchronous
/// call back into the identity service on every hop.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The subject of the access token, or <see cref="Guid.Empty"/> when unauthenticated.</summary>
    Guid Id { get; }

    bool IsAuthenticated { get; }

    /// <summary>Every house the token grants access to.</summary>
    IReadOnlyCollection<Guid> HouseIds { get; }

    bool CanAccess(Guid houseId);
}

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
