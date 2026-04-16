# REAL-WORLD CONTEXT — Pattern Matching in API Services

## Production Example: Appointment Validation and Scheduling

```csharp
#nullable enable

public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4,
    Rescheduled = 5
}

public class AppointmentService
{
    private readonly IAppointmentRepository _appointments;
    private readonly IDoctorRepository _doctors;
    
    public async Task<(bool Success, string Message)> RescheduleAsync(
        int appointmentId, 
        DateTime newTime)
    {
        try
        {
            var appointment = await _appointments.GetByIdAsync(appointmentId);
            
            // Pattern matching: is the appointment valid for rescheduling?
            if (appointment is not { Status: AppointmentStatus.Scheduled })
            {
                return (
                    Success: false, 
                    Message: "Can only reschedule scheduled appointments"
                );
            }
            
            // Check doctor availability using property patterns
            var doctor = await _doctors.GetByIdAsync(appointment.DoctorId);
            if (doctor is null or { LicenseExpiryDate: < DateTime.UtcNow })
            {
                return (
                    Success: false, 
                    Message: "Doctor unavailable or license expired"
                );
            }
            
            // Validate new time
            if (newTime is < DateTime.UtcNow or > DateTime.UtcNow.AddMonths(6))
            {
                return (
                    Success: false, 
                    Message: "Appointment must be within 6 months"
                );
            }
            
            // All checks passed—reschedule
            appointment.ScheduledTime = newTime;
            appointment.Status = AppointmentStatus.Rescheduled;
            
            await _appointments.UpdateAsync(appointment);
            
            return (
                Success: true, 
                Message: $"Rescheduled to {newTime:g}"
            );
        }
        catch (DatabaseException ex)
        {
            // Log and return error tuple
            Console.WriteLine($"Database error: {ex.Message}");
            return (Success: false, Message: "Database error occurred");
        }
        catch (Exception ex)
        {
            // Unexpected error
            Console.WriteLine($"Unexpected error: {ex}");
            throw;  // Let higher-level handler deal with it
        }
    }
    
    public string GetStatusDisplay(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "📅 Scheduled",
            AppointmentStatus.Completed => "✅ Completed",
            AppointmentStatus.Cancelled => "❌ Cancelled",
            AppointmentStatus.NoShow => "⏭️ No Show",
            AppointmentStatus.Rescheduled => "🔄 Rescheduled",
            _ => "❓ Unknown"
        };
    }
}
```

### What This Teaches

1. **Tuples for return values:** Instead of throwing exceptions or wrapping results in an `ApiResponse` object, return `(bool, string)` for simple operations.

2. **Pattern matching for validation:** `is not { Status: Scheduled }` is far cleaner than `if (appointment == null || appointment.Status != Scheduled)`.

3. **Null-conditional + pattern matching:** `doctor is null or { LicenseExpiryDate: < DateTime.UtcNow }` handles both "null doctor" and "expired license" in one readable line.

4. **Switch expressions for enums:** Convert enums to display strings exhaustively, with no risk of missing a case.

5. **Selective exception handling:** Catch specific exceptions (DatabaseException) separately from unexpected errors. Re-throw unexpected exceptions so higher layers (middleware) can log them properly.

---

## Gotcha: Pattern Matching and Side Effects

```csharp
// ❌ This is confusing
if (appointment is { Status: AppointmentStatus.Scheduled })
{
    appointment.Status = AppointmentStatus.Completed;  // Side effect inside pattern check
}

// ✅ Clear separation
var isScheduled = appointment.Status == AppointmentStatus.Scheduled;
if (isScheduled)
{
    appointment.Status = AppointmentStatus.Completed;
}
```

Pattern matching is for **reading** structure, not for **modifying** state. Keep them separate.

---

## Another Gotcha: Forgetting the Catch-All (`_`)

```csharp
// ❌ Compiler warning: not exhaustive
var message = status switch
{
    AppointmentStatus.Scheduled => "Scheduled",
    AppointmentStatus.Completed => "Completed"
    // Missing Cancelled, NoShow, Rescheduled
};

// ✅ Exhaustive
var message = status switch
{
    AppointmentStatus.Scheduled => "Scheduled",
    AppointmentStatus.Completed => "Completed",
    _ => "Other status"
};
```

Always use `_` (catch-all) unless you have a specific reason not to.

---

## Code Review: Common Mistakes

### ❌ Throwing Generic Exceptions

```csharp
if (appointmentId <= 0)
{
    throw new Exception("Invalid appointment ID");  // Too generic
}
```

**Better:**

```csharp
if (appointmentId <= 0)
{
    throw new ArgumentException("Appointment ID must be positive", nameof(appointmentId));
}
```

### ❌ Nested Pattern Matching Without Readability

```csharp
// Unreadable
if (apt is { Doctor: { LicenseExpiryDate: not null, LicenseExpiryDate: > DateTime.UtcNow } })
{
    // ...
}

// Better: Extract to a method
if (IsDoctorLicenseValid(apt.Doctor))
{
    // ...
}

private bool IsDoctorLicenseValid(Doctor? doctor)
{
    return doctor is { LicenseExpiryDate: > DateTime.UtcNow };
}
```

### ❌ Catching Base Exception Without Re-throw

```csharp
try
{
    await appointmentService.RescheduleAsync(id, newTime);
}
catch (Exception ex)
{
    logger.LogError("Error: {message}", ex.Message);
    // Silently swallowed—caller gets no result, no exception
}

// Better: Return a result or re-throw
try
{
    await appointmentService.RescheduleAsync(id, newTime);
}
catch (DoctorNotAvailableException ex)
{
    logger.LogWarning("Doctor not available: {message}", ex.Message);
    throw;  // Let middleware handle it, or return error
}
```

---

## Summary

In a real clinic API, pattern matching and switch expressions replace complex if-else chains. Tuples replace wrapper objects. Custom exceptions and selective catching make error handling explicit. Type inference with `var` keeps code readable. Enums with extension methods encode domain logic. All of this together creates a backend that's fast to write, easy to read, and hard to break.
