# CORE CONCEPTS — Properties, Nullability, and Value Types

## 1. Auto-Properties vs. Backing Fields

### Auto-Property (Most Common)

```csharp
public class Doctor
{
    public string Name { get; set; }
    public int LicenseNumber { get; set; }
}
```

C# compiler generates a hidden backing field (`<Name>k__BackingField`) behind the scenes. You write one line; the compiler does the work.

**Equivalent in Kotlin:**
```kotlin
class Doctor {
    var name: String = ""
    var licenseNumber: Int = 0
}
```

### Auto-Property with Init-Only Setter

```csharp
public class Patient
{
    public string Email { get; init; }
    public string PhoneNumber { get; set; }
}
```

`init` means the property can only be set **during object initialization**, then it becomes read-only. Kotlin doesn't have direct equivalent; Kotlin uses `val` (immutable) vs `var` (mutable).

```csharp
var patient = new Patient { Email = "john@clinic.com", PhoneNumber = "123-456-7890" };
patient.Email = "other@clinic.com"; // ❌ Compile error: init-only property
patient.PhoneNumber = "999-999-9999"; // ✅ OK
```

### Backing Field with Logic

```csharp
private string _firstName;

public string FirstName
{
    get { return _firstName; }
    set { _firstName = value.Trim(); }
}
```

The underscore prefix (`_firstName`) is the backing field. The property `FirstName` is the public interface. The `set` accessor trims whitespace—logic that a simple auto-property can't express.

**Expression-bodied form (C# 6+):**
```csharp
private string _firstName;

public string FirstName
{
    get => _firstName;
    set => _firstName = value.Trim();
}
```

### Read-Only Property

```csharp
public class Appointment
{
    private readonly DateTime _createdAt = DateTime.UtcNow;
    
    public DateTime CreatedAt => _createdAt; // Getter only, no setter
}
```

`=>` (expression-bodied) is shorthand for a property that returns a value with no setter. Only the constructor or initialization can set the backing field.

---

## 2. Nullable Reference Types (NRT)

### What It Is

C# 8+ allows you to declare intent: **is this reference allowed to be null?**

```csharp
#nullable enable  // Turn on nullable reference types for this file

public class Doctor
{
    public string Name { get; set; } = string.Empty;           // ✅ Non-null (must have a value)
    public string? MiddleName { get; set; }                     // ✅ Nullable (may be null)
    public string? Bio { get; set; } = null;                    // ✅ Explicit null assignment
    public List<string> Specialties { get; set; } = new();      // ✅ Non-null collection
    public List<string>? Tags { get; set; }                     // ✅ Nullable collection
}
```

**Kotlin equivalent:**
```kotlin
class Doctor {
    val name: String                       // Non-null
    val middleName: String?                // Nullable
    val bio: String? = null
    val specialties: List<String>          // Non-null
    val tags: List<String>?                // Nullable
}
```

### The Compiler Checks This

```csharp
#nullable enable

public class PatientService
{
    public string GetEmail(Patient patient)
    {
        return patient.Email; // ⚠️ Warning: Email is nullable, return type is non-null
    }
}

var service = new PatientService();
string email = service.GetEmail(patient); // ⚠️ Warning: may receive null
```

**Fix:** Either make the return type nullable or check for null:

```csharp
// Option 1: Return nullable
public string? GetEmail(Patient patient)
{
    return patient.Email;
}

// Option 2: Assert it's not null (your responsibility)
public string GetEmail(Patient patient)
{
    return patient.Email ?? throw new InvalidOperationException("Email is required");
}
```

### Nullable Value Types (int?, DateTime?)

Reference types use `?` suffix. Value types (int, bool, DateTime) are always non-null. To make them nullable, use `?`:

```csharp
public class Appointment
{
    public int AppointmentId { get; set; }           // int, never null
    public int? DoctorId { get; set; }               // int?, may be null
    public DateTime ScheduledTime { get; set; }      // DateTime, never null
    public DateTime? CompletedTime { get; set; }     // DateTime?, may be null
}
```

**Why?** An `int` is a value type stored on the stack—it always has *some* value (0, 1, etc.). Nullable value types are wrapped in a `Nullable<T>` struct, which adds a `bool HasValue` flag.

```csharp
int? doctorId = null;
if (doctorId.HasValue)
{
    Console.WriteLine($"Doctor ID: {doctorId.Value}");
}
else
{
    Console.WriteLine("No doctor assigned");
}
```

---

## 3. Null-Coalescing and Null-Conditional Operators

### Null-Coalescing (`??`)

```csharp
string displayName = patient.MiddleName ?? patient.FirstName;
// If MiddleName is null, use FirstName
```

### Null-Coalescing Assignment (`??=`)

```csharp
patient.PhoneNumber ??= "000-000-0000"; // Assign default only if currently null
```

### Null-Conditional (`?.`)

```csharp
string? middleInitial = patient.MiddleName?.FirstOrDefault()?.ToString();
// If MiddleName is null, the whole expression is null (no NullReferenceException)
```

**Kotlin equivalent:**
```kotlin
val middleInitial = patient.middleName?.get(0)?.toString()
```

---

## 4. String Handling

### Interpolation (`$""`)

```csharp
string name = "Alice";
string message = $"Welcome, {name}!";           // "Welcome, Alice!"
string formatted = $"Price: ${100.5:C}";       // "Price: $100.50"
```

### Verbatim Strings (`@""`)

```csharp
string path = @"C:\Users\Data\file.txt";       // Backslashes NOT escaped
string json = @"{ ""name"": ""Alice"" }";      // Double quotes are literal
```

### Raw String Literals (C# 11+)

```csharp
string json = """
{
    "name": "Alice",
    "email": "alice@clinic.com"
}
""";  // No escaping needed
```

---

## 5. `record` Types (C# 9+)

### What It Is

A `record` is a reference type optimized for immutability and value-based equality. It's C#'s answer to Kotlin `data class`.

```csharp
public record Doctor(int Id, string Name, string Specialty)
{
    public override string ToString() => $"Dr. {Name} ({Specialty})";
}

var doc1 = new Doctor(1, "Alice", "Cardiology");
var doc2 = new Doctor(1, "Alice", "Cardiology");

Console.WriteLine(doc1 == doc2); // ✅ true (value-based equality, not reference)
```

**Kotlin equivalent:**
```kotlin
data class Doctor(val id: Int, val name: String, val specialty: String)
```

### Mutable Record (Less Common)

```csharp
public record Patient
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string? Email { get; set; }  // Can be mutated after init
}
```

### `with` Expression (Copy with Changes)

```csharp
var patient1 = new Patient { Id = 1, Name = "Bob", Email = "bob@clinic.com" };
var patient2 = patient1 with { Email = "newemail@clinic.com" };

Console.WriteLine(patient1.Email); // "bob@clinic.com" (unchanged)
Console.WriteLine(patient2.Email); // "newemail@clinic.com" (new instance)
```

---

## 6. Value Types vs. Reference Types

### Reference Types (class, record, string)

```csharp
public class Doctor { }

var doc1 = new Doctor();
var doc2 = doc1;

doc2 = null;
Console.WriteLine(doc1 == null); // false (doc1 and doc2 are different references)
```

Stored on the heap. Assignment copies the **reference**, not the data.

### Value Types (struct, int, bool, DateTime)

```csharp
public struct Coordinate
{
    public double X { get; set; }
    public double Y { get; set; }
}

var coord1 = new Coordinate { X = 1.0, Y = 2.0 };
var coord2 = coord1;

coord2.X = 99.0;
Console.WriteLine(coord1.X); // 1.0 (coord1 unaffected—coord2 is a copy)
```

Stored on the stack. Assignment copies the **data**.

### When to Use `struct`

- Small, immutable data (< 16 bytes)
- Performance-critical code where heap allocation matters
- **Avoid** mutable structs—they cause subtle bugs

### Default Values

```csharp
int x = default;              // 0
bool b = default;             // false
string s = default;           // null
DateTime dt = default;        // January 1, 0001
Doctor doc = default;         // null
```

---

## ASCII Diagram: Property Flow

```
┌─────────────────────────────────────────────────────┐
│  Public API (Property)                              │
│  public string Name { get; set; }                   │
└────────────┬──────────────────────────────┬─────────┘
             │ Getter                       │ Setter
             ▼                              ▼
      ┌──────────────┐              ┌──────────────┐
      │ Return value │              │ Store value  │
      │ from backing │              │ in backing   │
      │ field        │              │ field        │
      └──────┬───────┘              └──────┬───────┘
             │                             │
             └──────────────┬──────────────┘
                            ▼
                  ┌─────────────────────┐
                  │ Backing Field       │
                  │ private string _name│
                  └─────────────────────┘
```

---

## Summary Table: Property Syntax at a Glance

| Syntax | Behavior | Backing Field |
|---|---|---|
| `public string Name { get; set; }` | Read/write | Auto-generated |
| `public string Name { get; }` | Read-only | Auto-generated |
| `public string Name { get; init; }` | Read, write on init only | Auto-generated |
| `public string Name => _name;` | Read-only, expression-bodied | Manual `_name` |
| `public string? Name { get; set; }` | Nullable, read/write | Auto-generated |
