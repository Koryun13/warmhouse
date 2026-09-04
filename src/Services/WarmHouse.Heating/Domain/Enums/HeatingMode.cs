namespace WarmHouse.Heating.Domain.Enums;

/// <summary>How a zone is being driven.</summary>
public enum HeatingMode
{
    /// <summary>Heating is off.</summary>
    Off = 0,

    /// <summary>The user drives the setpoint, as in the As-Is system.</summary>
    Manual = 1,

    /// <summary>The service holds the target temperature by itself.</summary>
    Auto = 2,
}
