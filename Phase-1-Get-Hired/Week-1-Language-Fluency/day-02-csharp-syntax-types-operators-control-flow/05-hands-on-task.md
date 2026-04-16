# HANDS-ON TASK — Build Clinic Service with Pattern Matching and Enums

## Task

Extend your clinic domain from Day 1 by adding service logic that uses pattern matching, enums, and tuples. Build an `AppointmentService` that validates and processes appointments using modern C# syntax.

Your service must:
1. Define an `AppointmentStatus` enum with at least 4 states
2. Use pattern matching to validate appointment rescheduling
3. Return tuples `(bool Success, string Message)` from operations
4. Use switch expressions to convert enums to display strings
5. Implement custom exception handling for business logic errors
6. Use `var` for type inference where appropriate

---

## Files to Create / Modify

- `Domain/AppointmentStatus.cs` — Enum with appointment states
- `Domain/Appointment.cs` — Add `Status` property and computed status methods
- `Services/AppointmentService.cs` — Validation and rescheduling logic using pattern matching
- `Program.cs` — Add minimal service wiring and test scenario

---

## Expected Output / Behavior

When you run the program, you should be able to:

```csharp
var doctor = new Doctor { /* ... */ };
var patient = new Patient { /* ... */ };

var appointment = new Appointment
{
    Id = 1,
    Doctor = doctor,
    Patient = patient,
    Status = AppointmentStatus.Scheduled,
    ScheduledTime = DateTime.UtcNow.AddDays(7)
};

var service = new AppointmentService();

// Reschedule succeeds
var (success, msg) = service.ValidateReschedule(appointment, DateTime.UtcNow.AddDays(14));
Console.WriteLine(success);  // true
Console.WriteLine(msg);      // "Valid: can reschedule"

// Reschedule fails (appointment not scheduled)
appointment.Status = AppointmentStatus.Completed;
(success, msg) = service.ValidateReschedule(appointment, DateTime.UtcNow.AddDays(14));
Console.WriteLine(success);  // false
Console.WriteLine(msg);      // "Cannot reschedule completed appointment"

// Get status display
Console.WriteLine(service.GetStatusDisplay(AppointmentStatus.Scheduled));  // "📅 Scheduled"
```

**No compile warnings or errors** when `dotnet build` runs.

---

## Do NOT Do This

- **No unhandled nulls in pattern matching:** If you use `is { Doctor: ... }`, ensure the Doctor reference is checked for null first or use null-conditional (`?.`).

- **Don't forget exhaustiveness in switch expressions:** If using an enum in a switch expression, either handle all cases or include `_ => ...`.

- **Don't return generic tuples from public APIs:** Tuples like `(bool, string, int, int, string)` are hard to read. Use descriptive names: `(bool Success, string Message)`.

- **Don't silently swallow exceptions:** Every `catch` block must either log, return an error, or re-throw. Never catch and do nothing.

- **Don't use `var` for unclear types:** If reading the code and you can't tell the type without hovering, use explicit types.

---

## Self-Review Checklist

- [ ] `AppointmentStatus` enum created with at least 4 states (Scheduled, Completed, Cancelled, etc.)
- [ ] `Appointment` class has `Status` property of type `AppointmentStatus`
- [ ] `AppointmentService` class created with at least two public methods
- [ ] `ValidateReschedule(Appointment apt, DateTime newTime)` uses pattern matching to check if appointment is scheduled
- [ ] Returns tuple `(bool Success, string Message)` with descriptive names
- [ ] `GetStatusDisplay(AppointmentStatus status)` uses a switch expression to map enum to display string
- [ ] At least one custom exception defined (e.g., `InvalidAppointmentException`)
- [ ] Pattern matching uses `is not`, `is { ... }`, or `is or` at least once
- [ ] No generic `catch (Exception)` without logging or re-throw
- [ ] `dotnet build` completes with zero warnings and zero errors
- [ ] You can explain which patterns are exhaustive and which aren't
