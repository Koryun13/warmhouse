namespace WarmHouse.Heating.Application.Contracts.Requests;

/// <summary>The requester is the authenticated caller, taken from the token.</summary>
public sealed record SetSetpointRequest(double TargetTemperature);
