# REAL-WORLD CONTEXT — Domain Models in the Clinic API

## Production Example: Doctor Model

```csharp
#nullable enable

public class Doctor
{
    // Primary key
    public int Id { get; init; }
    
    // Immutable during creation (init), mutable after (set)
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    
    // Optional field (nullable reference type)
    public string? MiddleName { get; init; }
    
    // Nullable value type: appointment may have no end time if ongoing
    public DateTime? LicenseExpiryDate { get; set; }
    
    // Read-only, set by constructor/init
    public string Specialty { get; init; } = string.Empty;
    
    // Derived property—no backing field, computed on-demand
    public string FullName => 
        string.IsNullOrEmpty(MiddleName) 
            ? $"{FirstName} {LastName}" 
            : $"{FirstName} {MiddleName} {LastName}";
    
    // Navigation property (EF Core will handle this)
    public List<Appointment> Appointments { get; set; } = new();
}
```

### What This Teaches

1. **`init` + required fields**: Once a Doctor is created, `FirstName`, `LastName`, and `Specialty` cannot change. This is **domain logic**: a doctor's identity is fixed.

2. **Nullable `MiddleName`**: Not every doctor has a middle name. The compiler enforces that code checking `MiddleName` must handle the null case.

3. **Nullable value type `LicenseExpiryDate?`**: A license may not have an expiry (unrealistic but illustrates the point). The type system forces you to ask "Is this date null?" before using it.

4. **Computed property `FullName`**: No backing field. Just logic. EF Core will **not** try to map this to the database.

---

## Gotcha: Uninitialized Non-Nullable Properties

```csharp
#nullable enable

public class Patient
{
    public string Email { get; set; }  // ⚠️ Warning: non-nullable, but no default value
}

var patient = new Patient();
// At runtime, Email is uninitialized (null), but the compiler warned you!
Console.WriteLine(patient.Email.Length);  // ❌ Potential NullReferenceException
```

**Fix:**

```csharp
public class Patient
{
    public string Email { get; set; } = string.Empty;  // Default value
    // OR
    public required string Email { get; set; }  // Must be provided in constructor
}
```

The `required` keyword (C# 11+) forces the caller to set this property during object initialization:

```csharp
var patient = new Patient { Email = "alice@clinic.com" };  // ✅ Compiles
var patient2 = new Patient();                               // ❌ Compile error
```

---

## Another Gotcha: Forgetting `#nullable enable`

If you don't enable nullable reference types, `string` and `string?` are treated the same:

```csharp
// Without #nullable enable
public class Doctor
{
    public string Name { get; set; }  // Compiler doesn't warn if you assign null
}

var doc = new Doctor();
doc.Name = null;  // No warning—treated as valid
```

**In production code:** Always enable NRT at the file or project level:

```xml
<!-- In .csproj -->
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

This forces the entire project to respect nullable reference types, catching potential null-reference bugs at compile time.

---

## Code Review: Common Mistakes

### ❌ Mutable Collection Navigation

```csharp
public class Doctor
{
    public List<Appointment> Appointments { get; set; } = new();
}
```

**Problem:** EF Core might get confused if the collection reference changes. You can modify the list's *contents*, but EF tracks the reference.

**Better:**

```csharp
public class Doctor
{
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
```

Or in EF Core with shadow properties, let EF manage the relationship entirely.

### ❌ Property with Side Effects in Hot Paths

```csharp
public class Appointment
{
    private int _notificationsSent;
    
    public int NotificationsSent
    {
        get { return _notificationsSent; }
        set 
        { 
            _notificationsSent = value;
            SendEmail("Appointment updated");  // ❌ Side effect in setter
        }
    }
}
```

**Problem:** Callers won't expect setting a property to trigger an email. This causes confusion and bugs.

**Better:** Use an explicit method:

```csharp
public void UpdateNotificationCount(int count)
{
    _notificationsSent = count;
    SendEmail("Appointment updated");
}
```

---

## Summary

In a real clinic API, domain models are the spine of your system. Properties define what data is immutable, what can change, what's optional, and what's required. Get this right from day one—it will save hours of refactoring later.
