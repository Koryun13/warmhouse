using WarmHouse.Heating.Domain.Enums;

namespace WarmHouse.Heating.Application.Contracts.Requests;

/// <summary>Switches a zone between off, manual and automatic control.</summary>
public sealed record SetModeRequest(HeatingMode Mode);
