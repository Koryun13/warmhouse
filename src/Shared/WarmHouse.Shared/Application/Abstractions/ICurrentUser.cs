namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// The authenticated caller of the current request.
///
/// Handlers take the identity from here and never from the request body: a
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
