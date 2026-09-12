namespace WarmHouse.Gates.Application.Contracts.Requests;

/// <summary>
/// The operation a caller asked for.
///
/// Gate operations carry no request body: the requester is the authenticated
/// caller, taken from the token, and the operation is part of the route.
/// </summary>
public enum GateOperation
{
    Open,
    Close,
    Lock,
}
