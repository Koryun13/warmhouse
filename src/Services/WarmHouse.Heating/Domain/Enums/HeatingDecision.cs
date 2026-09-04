namespace WarmHouse.Heating.Domain.Enums;

/// <summary>What the service should do after a new reading.</summary>
public enum HeatingDecision
{
    None,
    StartHeating,
    StopHeating,
}
