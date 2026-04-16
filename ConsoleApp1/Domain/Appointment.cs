#nullable enable // Enable nullable reference types for static analysis

namespace ConsoleApp1.Domain;

public class Appointment
{
    public int Id { get; init; }
    public int DoctorId { get; init; }
    // Changed from string to int to match the task's sample code (PatientId = 101)
    public int PatientId { get; init; }
    public DateTime ScheduledTime { get; set; }
    // Optional: can be updated (e.g., patient adds more details later)
    public string ReasonForVisit { get; set; } = string.Empty;
    // Nullable: appointment may be rescheduled; null means no reschedule yet
    public DateTime? RescheduledTime { get; set; }
    // Appointment status: tracks lifecycle state
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    // Computed method: checks if the appointment is in the future (matches task's IsUpcoming() call)
    public bool IsUpcoming() => ScheduledTime > DateTime.UtcNow;
}
