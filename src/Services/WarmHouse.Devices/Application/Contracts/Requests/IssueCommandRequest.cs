namespace WarmHouse.Devices.Application.Contracts.Requests;

/// <summary>A command addressed to a device by capability rather than by model.</summary>
public sealed record IssueCommandRequest(
    string Capability,
    string Action,
    Dictionary<string, string>? Payload);
