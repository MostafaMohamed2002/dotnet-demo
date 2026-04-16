#nullable enable

using ConsoleApp1.Domain;

namespace ConsoleApp1.Services;

/// <summary>
/// Service for validating and processing appointment operations.
/// Uses pattern matching, switch expressions, and tuple returns for modern C# idioms.
/// </summary>
public class AppointmentService
{
    /// <summary>
    /// Validates whether an appointment can be rescheduled to a new time.
    /// Uses pattern matching to check appointment state and business rules.
    /// </summary>
    public (bool Success, string Message) ValidateReschedule(Appointment appointment, DateTime newTime)
    {
        // Pattern matching: check if appointment is null or has invalid state
        if (appointment is null)
        {
            return (false, "Appointment cannot be null");
        }

        // Pattern matching with property patterns: check appointment state
        var result = appointment switch
        {
            // Cannot reschedule appointments that are already in final state
            { Status: AppointmentStatus.Completed } 
                => (false, "Cannot reschedule completed appointment"),
            
            { Status: AppointmentStatus.Cancelled } 
                => (false, "Cannot reschedule cancelled appointment"),
            
            { Status: AppointmentStatus.NoShow } 
                => (false, "Cannot reschedule no-show appointment"),

            // Can only reschedule appointments that are scheduled
            { Status: AppointmentStatus.Scheduled }
                => ValidateNewTime(appointment, newTime),

            // Catch-all (should not reach here if enum is exhaustive, but defensive)
            _ => (false, "Invalid appointment status")
        };

        return result;
    }

    /// <summary>
    /// Private helper: validates the new appointment time against business rules.
    /// Uses null-conditional operator and pattern matching.
    /// </summary>
    private (bool Success, string Message) ValidateNewTime(Appointment appointment, DateTime newTime)
    {
        // Pattern matching: new time must be in future and not before original time
        return newTime switch
        {
            // New time is in the past
            var time when time <= DateTime.UtcNow 
                => (false, "Cannot reschedule to a past time"),

            // New time is before the original appointment
            var time when time < appointment.ScheduledTime 
                => (false, "New time cannot be before the original appointment"),

            // New time is valid
            var time when time > appointment.ScheduledTime 
                => (true, "Valid: can reschedule"),

            // Should not reach here, but defensive
            _ => (false, "Invalid new time")
        };
    }

    /// <summary>
    /// Converts an appointment status enum to a display-friendly string.
    /// Uses switch expression for exhaustive pattern matching on enum.
    /// </summary>
    public string GetStatusDisplay(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "📅 Scheduled",
            AppointmentStatus.Completed => "✅ Completed",
            AppointmentStatus.Cancelled => "❌ Cancelled",
            AppointmentStatus.NoShow => "⏸️ No Show",
            
            // Catch-all for any future enum values
            _ => "❓ Unknown"
        };
    }

    /// <summary>
    /// Checks if an appointment is in a final state (cannot be modified).
    /// Demonstrates pattern matching with OR operator.
    /// </summary>
    public bool IsFinal(Appointment? appointment)
    {
        if (appointment is null)
        {
            return false;
        }

        // Pattern matching with OR: check if status is any final state
        return appointment.Status is AppointmentStatus.Completed 
            or AppointmentStatus.Cancelled 
            or AppointmentStatus.NoShow;
    }

    /// <summary>
    /// Attempts to cancel an appointment.
    /// Demonstrates exception handling and tuple returns.
    /// </summary>
    public (bool Success, string Message) TryCancel(Appointment appointment, string? reason = null)
    {
        try
        {
            if (appointment is null)
            {
                throw new InvalidAppointmentException("Appointment cannot be null");
            }

            // Use null-coalescing to provide default reason
            var cancelReason = reason ?? "Cancelled by system";

            // Pattern matching: can only cancel if appointment is scheduled
            if (appointment.Status is not AppointmentStatus.Scheduled)
            {
                return (false, $"Cannot cancel appointment with status: {GetStatusDisplay(appointment.Status)}");
            }

            // Update appointment status
            appointment.Status = AppointmentStatus.Cancelled;

            return (true, $"Appointment cancelled: {cancelReason}");
        }
        catch (InvalidAppointmentException ex)
        {
            return (false, $"Invalid operation: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Unexpected error: {ex.Message}");
        }
    }
}
