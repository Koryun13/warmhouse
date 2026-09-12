namespace WarmHouse.Lighting.Application.Contracts.Requests;

/// <summary>Dimming level, valid only for a fixture that declares it can dim.</summary>
public sealed record SetBrightnessRequest(int Brightness);
