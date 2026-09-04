namespace WarmHouse.Identity.Application.Contracts.Requests;

/// <summary>The owner is the authenticated caller, never a value from the body.</summary>
public sealed record CreateHouseRequest(string Name, string? Address, string? TimeZone);
