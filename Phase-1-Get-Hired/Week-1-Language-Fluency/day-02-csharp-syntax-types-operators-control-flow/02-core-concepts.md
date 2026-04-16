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

Returns the left operand if it's **not null**, otherwise the right operand.

```csharp
string displayName = patient.MiddleName ?? patient.FirstName ?? "Unknown";
```

**Traced execution:**

```csharp
var patient = new Patient { FirstName = "Alice", MiddleName = null };

// Step 1: Check patient.MiddleName
// patient.MiddleName = null  →  Check next

// Step 2: Check patient.FirstName
// patient.FirstName = "Alice"  →  Use this

string displayName = "Alice";
```

**Another trace:**

```csharp
var patient = new Patient { FirstName = "Bob", MiddleName = "James" };

// Step 1: Check patient.MiddleName
// patient.MiddleName = "James"  →  Use this (not null, stop evaluation)

string displayName = "James";
// Note: patient.FirstName is NEVER evaluated (short-circuit)
```

**Practical example with clinic data:**

```csharp
var patient = new Patient 
{ 
    Email = null,
    FallbackEmail = "backup@clinic.com",
    DefaultEmail = "noreply@clinic.com"
};

// Uses first non-null value
string notificationEmail = patient.Email 
    ?? patient.FallbackEmail 
    ?? patient.DefaultEmail 
    ?? throw new InvalidOperationException("No email available");

Console.WriteLine(notificationEmail);  // "backup@clinic.com"
```

---

### Null-Coalescing Assignment (`??=`)

Only assigns if the left operand is **null**.

```csharp
patient.PhoneNumber ??= "000-000-0000";
```

**Traced execution:**

```csharp
// Case 1: PhoneNumber is null
var patient = new Patient { PhoneNumber = null };

// Is PhoneNumber null?  YES  →  Assign default
patient.PhoneNumber ??= "000-000-0000";
// Result: PhoneNumber = "000-000-0000"


// Case 2: PhoneNumber already has a value
var patient = new Patient { PhoneNumber = "555-1234" };

// Is PhoneNumber null?  NO  →  Skip assignment
patient.PhoneNumber ??= "000-000-0000";
// Result: PhoneNumber = "555-1234" (unchanged)
```

**Real-world usage:**

```csharp
public void EnsurePatientHasPhoneNumber(Patient patient)
{
    // Only set default if patient doesn't already have a phone
    patient.PrimaryPhone ??= "000-000-0000";
    patient.SecondaryPhone ??= patient.PrimaryPhone;  // Reuse primary as secondary default
    
    Console.WriteLine($"Primary: {patient.PrimaryPhone}, Secondary: {patient.SecondaryPhone}");
}

// Trace 1: New patient with no phone
var newPatient = new Patient { PrimaryPhone = null, SecondaryPhone = null };
EnsurePatientHasPhoneNumber(newPatient);
// Output: "Primary: 000-000-0000, Secondary: 000-000-0000"

// Trace 2: Patient with existing phone
var existingPatient = new Patient { PrimaryPhone = "555-9999", SecondaryPhone = null };
EnsurePatientHasPhoneNumber(existingPatient);
// Output: "Primary: 555-9999, Secondary: 555-9999"
```

---

### Ternary Operator

Conditional expression: `condition ? valueIfTrue : valueIfFalse`

```csharp
string status = appointment.Status == AppointmentStatus.Completed 
    ? "Completed" 
    : "Pending";
```

**Traced execution:**

```csharp
// Case 1: Status is Completed
var appointment = new Appointment { Status = AppointmentStatus.Completed };

// Evaluate condition: appointment.Status == AppointmentStatus.Completed
// Result: TRUE  →  Use first branch

string status = "Completed";


// Case 2: Status is Scheduled
var appointment = new Appointment { Status = AppointmentStatus.Scheduled };

// Evaluate condition: appointment.Status == AppointmentStatus.Scheduled
// Result: FALSE  →  Use second branch

string status = "Pending";
```

**Nested ternary (not recommended, use switch instead):**

```csharp
var appointment = new Appointment { Status = AppointmentStatus.NoShow };

string message = appointment.Status == AppointmentStatus.Scheduled 
    ? "Scheduled" 
    : appointment.Status == AppointmentStatus.Completed 
        ? "Completed" 
        : appointment.Status == AppointmentStatus.Cancelled 
            ? "Cancelled" 
            : appointment.Status == AppointmentStatus.NoShow
                ? "No Show"
                : "Unknown";

// Result: "No Show"

// ✅ Better: Use switch expression
string message = appointment.Status switch
{
    AppointmentStatus.Scheduled => "Scheduled",
    AppointmentStatus.Completed => "Completed",
    AppointmentStatus.Cancelled => "Cancelled",
    AppointmentStatus.NoShow => "No Show",
    _ => "Unknown"
};
```

---

### Null-Conditional Operator (`?.`)

Safely accesses members of potentially-null objects. Returns **null** instead of throwing NullReferenceException.

```csharp
string? city = patient?.Address?.City;
```

**Traced execution:**

```csharp
// Case 1: Patient is not null, Address is not null
var patient = new Patient 
{ 
    Address = new Address { City = "New York" } 
};

// Step 1: Is patient null?  NO  →  Access Address
// Step 2: Is Address null?  NO  →  Access City
string? city = "New York";


// Case 2: Patient is not null, but Address is null
var patient = new Patient { Address = null };

// Step 1: Is patient null?  NO  →  Try to access Address
// Step 2: Is Address null?  YES  →  Stop (return null, don't access City)
string? city = null;


// Case 3: Patient itself is null
Patient? patient = null;

// Step 1: Is patient null?  YES  →  Stop immediately (return null)
// The .Address?.City is NEVER evaluated
string? city = null;
```

**Practical clinic example:**

```csharp
public string GetPatientCity(Patient? patient)
{
    // Safely navigates through potentially-null chain
    return patient?.Address?.City ?? "City not available";
}

// Trace 1: Patient is null
Patient? patient = null;
string city = GetPatientCity(patient);
// patient?.Address  →  null (short-circuit)
// ?? "City not available"  →  Use default
// Result: "City not available"


// Trace 2: Patient exists, Address is null
var patient = new Patient { Address = null };
string city = GetPatientCity(patient);
// patient?.Address  →  null 
// ?? "City not available"  →  Use default
// Result: "City not available"


// Trace 3: Full chain exists
var patient = new Patient 
{ 
    Address = new Address { City = "Boston" } 
};
string city = GetPatientCity(patient);
// patient?.Address?.City  →  "Boston"
// ?? "City not available"  →  Left side not null, use it
// Result: "Boston"
```

**With method calls:**

```csharp
public class Appointment
{
    public DateTime? ScheduledTime { get; set; }
    public Patient? AssignedPatient { get; set; }
    
    public string GetScheduledTimeDisplay()
    {
        // Safely call method on potentially-null property
        return ScheduledTime?.ToString("g") ?? "Not scheduled";
    }
    
    public string GetPatientEmail()
    {
        // Safely chain with method calls
        return AssignedPatient?.GetContactEmail() ?? "No contact info";
    }
}

// Traces:
var apt1 = new Appointment { ScheduledTime = new DateTime(2024, 12, 25, 14, 30, 0) };
string display1 = apt1.GetScheduledTimeDisplay();
// ScheduledTime != null  →  Call ToString("g")
// Result: "12/25/2024 2:30 PM"

var apt2 = new Appointment { ScheduledTime = null };
string display2 = apt2.GetScheduledTimeDisplay();
// ScheduledTime == null  →  Return null (don't call ToString)
// ?? "Not scheduled"  →  Use default
// Result: "Not scheduled"
```

---

## Comparison: `?.` vs `??` vs `?:`

| Operator | Purpose | Example | Behavior |
|----------|---------|---------|----------|
| `?.` | Safe member access | `obj?.Property` | Returns null if obj is null |
| `??` | Null coalescing | `x ?? y` | Returns y if x is null |
| `?:` | Ternary conditional | `x ? y : z` | Returns y if x true, else z |

**Combined in practice:**

```csharp
// Safe access (?.`) + coalesce (`??`) + conditional (`?:`)
string result = patient?.Address?.City 
    ?? (patient?.Email != null ? "Has email" : "No contact info");

// Trace:
var patient = new Patient { Address = null, Email = "test@clinic.com" };

// Step 1: patient?.Address?.City  →  null (Address is null)
// Step 2: ?? checks left side  →  null, evaluate right side
// Step 3: patient?.Email != null  →  "test@clinic.com" != null  →  TRUE
// Step 4: ternary returns "Has email"
// Result: "Has email"
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
