# CORE CONCEPTS — Type Inference, Pattern Matching, Tuples, and Exceptions

## 1. Type Inference with `var`

### Auto-Type Detection

```csharp
var name = "Alice";              // string (inferred)
var age = 30;                    // int (inferred)
var price = 99.99;               // double (inferred)
var isActive = true;             // bool (inferred)
var patients = new List<Patient>();  // List<Patient> (inferred)
```

C# infers the type at compile time. `var` is **not** dynamic—it's statically typed, just with the type deduced. This is similar to Kotlin's type inference, but with one key difference:

```csharp
// Kotlin: Type is inferred, can be shadowed with worse type
val x = "hello"  // String
val x = 42       // Int (shadows, allowed in some scopes)

// C#: var is strictly typed once assigned
var x = "hello";  // string
x = 42;           // ❌ Compile error: int is not assignable to string
```

### When to Use `var`

**Use `var` when the type is obvious:**

```csharp
var doctor = new Doctor { FirstName = "Alice", LastName = "Smith" };
var count = appointments.Count;
var message = $"Appointment on {date:dd/MM/yyyy}";
```

**Avoid `var` when the type is unclear:**

```csharp
// ❌ What type is result?
var result = GetAppointmentData(patientId);

// ✅ Explicit type is clearer
AppointmentData result = GetAppointmentData(patientId);
```

---

## 2. Pattern Matching

### Type Patterns

```csharp
public string ValidateAppointment(object obj)
{
    return obj switch
    {
        Appointment apt => $"Valid appointment on {apt.ScheduledTime}",
        string id => $"Received string ID: {id}",
        int appointmentId => $"Received int ID: {appointmentId}",
        null => "Null object provided",
        _ => "Unknown type"  // Catch-all
    };
}
```

This is C#'s version of Kotlin `when`:

```kotlin
val result = when (obj) {
    is Appointment -> "Valid appointment"
    is String -> "Received string ID: $obj"
    is Int -> "Received int ID: $obj"
    null -> "Null object"
    else -> "Unknown type"
}
```

### Property Patterns

```csharp
public bool IsLicenseValid(Doctor doctor)
{
    return doctor is 
    { 
        LicenseExpiryDate: not null,
        LicenseExpiryDate: > DateTime.UtcNow
    };
}

// Compact form:
var isValid = doctor is { LicenseExpiryDate: > DateTime.UtcNow };
```

**Real-world example:**

```csharp
public decimal CalculateAppointmentCost(Appointment apt)
{
    return apt switch
    {
        // Urgent appointment, after hours
        { IsUrgent: true, ScheduledTime.Hour: > 18 } => 200m,
        
        // Regular appointment
        { IsUrgent: false } => 100m,
        
        // No appointment
        null => throw new ArgumentNullException(nameof(apt)),
        
        _ => 100m
    };
}
```

### Pattern Combinators (C# 9+)

```csharp
// AND
if (patient is { FirstName: "Bob", Email: not null })
{
    // Both conditions must be true
}

// OR
if (patient is { FirstName: "Alice" } or { FirstName: "Bob" })
{
    // Either condition is true
}

// NOT
if (patient is not null)
{
    // Equivalent to patient != null
}
```

---

## 3. String Handling: Interpolation and Formatting

### Basic Interpolation

```csharp
string name = "Alice";
int age = 30;

string message = $"Name: {name}, Age: {age}";
// "Name: Alice, Age: 30"
```

### Format Specifiers

```csharp
DateTime date = new DateTime(2024, 12, 25);
decimal price = 99.99m;

string formatted = $"Date: {date:yyyy-MM-dd}, Price: {price:C}";
// "Date: 2024-12-25, Price: $99.99"

string padded = $"ID: {patientId:D5}";  // "ID: 00123"
```

### Expressions in Interpolation

```csharp
List<Appointment> appointments = new() { /* ... */ };

string summary = $"Total: {appointments.Count} appointments";
string firstAppt = $"Next appointment: {appointments.FirstOrDefault()?.ScheduledTime:g}";
```

### Raw String Literals (C# 11+)

```csharp
string json = """
{
    "doctor": "Alice",
    "specialty": "Cardiology"
}
""";

string sql = """
SELECT doctor_id, COUNT(*) as appointment_count
FROM appointments
WHERE scheduled_time > @minDate
GROUP BY doctor_id
""";
```

No escaping needed. Indentation is preserved.

---

## 4. Tuples

### Returning Multiple Values

**Kotlin approach (using data class):**

```kotlin
data class Result(val success: Boolean, val message: String, val appointmentId: Int)

fun scheduleAppointment(...): Result {
    return Result(true, "Scheduled", 123)
}
```

**C# approach (using tuple):**

```csharp
public (bool Success, string Message, int AppointmentId) ScheduleAppointment(...)
{
    return (true, "Scheduled", 123);
}

// Calling code:
var result = ScheduleAppointment(...);
if (result.Success)
{
    Console.WriteLine($"Appointment {result.AppointmentId} confirmed");
}
```

### Tuple Deconstruction

```csharp
var (success, message, appointmentId) = ScheduleAppointment(...);

if (success)
{
    Console.WriteLine($"Appointment {appointmentId}: {message}");
}

// Discard unused values with _:
var (success, _, appointmentId) = ScheduleAppointment(...);
```

### Named Tuple Elements

```csharp
public (bool IsValid, string ErrorMessage) ValidateDoctor(Doctor doctor)
{
    if (string.IsNullOrEmpty(doctor.FirstName))
    {
        return (IsValid: false, ErrorMessage: "First name is required");
    }
    
    return (IsValid: true, ErrorMessage: "");
}
```

### When NOT to Use Tuples

Tuples are great for simple, short-lived returns. For domain entities, use classes/records:

```csharp
// ❌ Hard to read later
public (int Id, string Name, string Email, string Phone, string Specialty) GetDoctorInfo(int id)
{
    // ...
}

// ✅ Use a record or class
public record DoctorInfo(int Id, string Name, string Email, string Phone, string Specialty);

public DoctorInfo GetDoctorInfo(int id)
{
    // ...
}
```

---

## 5. Enums for Domain Logic

### Basic Enum

```csharp
public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}

public class Appointment
{
    public AppointmentStatus Status { get; set; }
}
```

### Enum with Logic

```csharp
public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}

public static class AppointmentStatusExtensions
{
    public static bool IsFinal(this AppointmentStatus status)
    {
        return status is AppointmentStatus.Completed 
            or AppointmentStatus.Cancelled 
            or AppointmentStatus.NoShow;
    }
    
    public static string DisplayName(this AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "Scheduled",
            AppointmentStatus.Completed => "Completed",
            AppointmentStatus.Cancelled => "Cancelled",
            AppointmentStatus.NoShow => "No Show",
            _ => "Unknown"
        };
    }
}
```

**Usage:**

```csharp
var apt = new Appointment { Status = AppointmentStatus.Scheduled };

if (apt.Status.IsFinal())
{
    Console.WriteLine("Cannot reschedule a final appointment");
}

Console.WriteLine($"Status: {apt.Status.DisplayName()}");  // "Status: Scheduled"
```

---

## 6. Exception Handling

### Try-Catch-Finally

```csharp
try
{
    var patient = patientRepository.GetById(patientId);
    if (patient is null)
    {
        throw new InvalidOperationException("Patient not found");
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Business logic error: {ex.Message}");
}
catch (DatabaseException ex)
{
    Console.WriteLine($"Database error: {ex.Message}");
    throw;  // Re-throw to let higher-level handler deal with it
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
finally
{
    // Runs regardless of exception
    Console.WriteLine("Cleanup here");
}
```

### Exception Filters (C# 6+)

```csharp
try
{
    await appointmentService.RescheduleAsync(appointmentId, newTime);
}
catch (ArgumentException ex) when (ex.ParamName == nameof(newTime))
{
    // Only handle ArgumentException if it's about newTime
    Console.WriteLine("Invalid new time provided");
}
catch (ArgumentException ex)
{
    // Handle other ArgumentException cases
    Console.WriteLine("Invalid argument");
}
```

### Custom Exceptions

```csharp
public class DoctorNotAvailableException : Exception
{
    public int DoctorId { get; }
    public DateTime RequestedTime { get; }
    
    public DoctorNotAvailableException(int doctorId, DateTime requestedTime, string message)
        : base(message)
    {
        DoctorId = doctorId;
        RequestedTime = requestedTime;
    }
}

// Usage:
try
{
    var slot = availabilityService.GetSlot(doctorId, time);
    if (slot is null)
    {
        throw new DoctorNotAvailableException(doctorId, time, "No slots available");
    }
}
catch (DoctorNotAvailableException ex)
{
    logger.LogWarning("Doctor {doctorId} has no slots at {time}", ex.DoctorId, ex.RequestedTime);
}
```

---

## 7. Null-Coalescing and Conditional Operators

### Null-Coalescing (`??`)

```csharp
string displayName = patient.MiddleName ?? patient.FirstName ?? "Unknown";
```

### Null-Coalescing Assignment (`??=`)

```csharp
patient.PhoneNumber ??= "000-000-0000";
// Only assign if null
```

### Ternary Operator

```csharp
string status = appointment.Status == AppointmentStatus.Completed 
    ? "Completed" 
    : "Pending";
```

### Null-Conditional Operator (`?.`)

```csharp
string? city = patient?.Address?.City;  // No NullReferenceException if patient or Address is null
```

---

## 8. Switch Expressions vs. If-Else

### Traditional If-Else

```csharp
string message;
if (status == AppointmentStatus.Scheduled)
{
    message = "Your appointment is scheduled";
}
else if (status == AppointmentStatus.Completed)
{
    message = "Appointment completed";
}
else
{
    message = "Unknown status";
}
```

### Switch Expression (C# 8+)

```csharp
string message = status switch
{
    AppointmentStatus.Scheduled => "Your appointment is scheduled",
    AppointmentStatus.Completed => "Appointment completed",
    _ => "Unknown status"
};
```

**Switch expressions are:**
- Cleaner (less boilerplate)
- Exhaustiveness-checked (compiler warns if you miss a case)
- Expression-based (can be assigned, returned, used in LINQ)

---

## ASCII Diagram: Pattern Matching Flow

```
┌─────────────────────────────────────────────┐
│  Input: object obj                          │
└────────────────────┬────────────────────────┘
                     │
         ┌───────────┴──────────┬─────────────────┬──────────┐
         ▼                      ▼                 ▼          ▼
    Appointment?           string?             int?      null/other
         │                      │                 │          │
    ┌────┴────┐           ┌─────┴─────┐    ┌────┴────┐ ┌───┴───┐
    ▼         ▼           ▼           ▼    ▼         ▼ ▼       ▼
"Valid appt" "String ID" "Int ID"  (null) (unknown)
         │                      │                 │          │
         └───────────────────────┼─────────────────┴──────────┘
                                 ▼
                      Return matched string
```

---

## Summary Table: C# Syntax Essentials

| Feature | Syntax | Use Case |
|---|---|---|
| Type inference | `var x = ...;` | When type is obvious |
| Type pattern | `obj is Appointment` | Type checking |
| Property pattern | `obj is { Name: "Bob" }` | Structural matching |
| Pattern combination | `obj is { X: > 10 } or { Y: < 0 }` | Complex conditions |
| String interpolation | `$"Value: {x:C}"` | Formatting |
| Raw strings | `""" multiline """` | JSON, SQL, templates |
| Tuples | `(bool, string, int)` | Multiple return values |
| Tuple deconstruction | `var (x, y, z) = tuple;` | Extracting tuple values |
| Enum | `enum Status { A, B }` | Domain constants |
| Switch expression | `x switch { A => 1, _ => 0 }` | Exhaustive matching |
| Try-catch-finally | `try { } catch { } finally { }` | Error handling |
| Custom exception | `class MyEx : Exception { }` | Domain-specific errors |
