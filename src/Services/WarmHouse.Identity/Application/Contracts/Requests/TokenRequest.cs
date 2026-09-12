namespace WarmHouse.Identity.Application.Contracts.Requests;

/// <summary>Credentials exchanged for an access token.</summary>
public sealed record TokenRequest(string Email, string Password);
