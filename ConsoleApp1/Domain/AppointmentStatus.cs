#nullable enable

namespace ConsoleApp1.Domain;

/// <summary>
/// Represents the lifecycle state of an appointment.
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}
