namespace WarmHouse.Monitoring.Application.Contracts.Requests;

/// <summary>The requester is the authenticated caller, taken from the token.</summary>
public sealed record SetRecordingRequest(bool Enabled);
