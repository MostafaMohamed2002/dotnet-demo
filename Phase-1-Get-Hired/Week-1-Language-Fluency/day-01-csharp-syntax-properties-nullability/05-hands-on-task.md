# HANDS-ON TASK — Build Clinic Domain Models with Proper Nullability

## Task

You are building the domain layer for a clinic appointment booking API. Create three entity classes (`Doctor`, `Patient`, `Appointment`) using C# properties, nullable reference types, and immutable-where-appropriate patterns.

Your models must:
1. Use `#nullable enable` at the top of each file
2. Distinguish between required and optional fields via `init`, `set`, and nullable types
3. Include computed/derived properties (no backing fields)
4. Pass static null-safety checks when compiled with `dotnet build`

---

## Files to Create / Modify

- `Domain/Doctor.cs` — Doctor entity with specialty, license expiry, full name computation
- `Domain/Patient.cs` — Patient entity with contact info and optional insurance
- `Domain/Appointment.cs` — Appointment entity with doctor/patient references and scheduling
- `Program.cs` — Add minimal setup to verify the models compile and work

---

## Expected Output / Behavior

When you run the project, you should be able to:

```csharp
var doctor = new Doctor 
{ 
    Id = 1, 
    FirstName = "Alice", 
    LastName = "Smith", 
    Specialty = "Cardiology",
    LicenseExpiryDate = new DateTime(2025, 12, 31)
};

var patient = new Patient
{
    Id = 101,
    FirstName = "Bob",
    LastName = "Johnson",
    Email = "bob@clinic.com",
    PhoneNumber = "555-1234"
};

var appointment = new Appointment
{
    Id = 1001,
    DoctorId = 1,
    PatientId = 101,
    ScheduledTime = DateTime.UtcNow.AddDays(7),
    ReasonForVisit = "Chest pain"
};

Console.WriteLine(doctor.FullName);              // "Alice Smith"
Console.WriteLine(appointment.IsUpcoming());     // true
```

**No compile warnings or errors** when `dotnet build` runs with nullable reference types enabled.

---

## Do NOT Do This

- **No backing fields without reason:** If you write `private string _name;`, it must be paired with logic in the property getter/setter. Otherwise, use auto-properties.
  
- **No uninitialized non-nullable properties:** Every `public string Name { get; set; }` must have a default value or use `= string.Empty` or `required string Name { get; set; }`.

- **Don't suppress nullability warnings:** Do NOT use `#pragma disable`, `!` (null-forgiving), or `#nullable disable` to hide warnings. If the compiler warns, fix the code.

- **No `ref` or `out` in domain models:** Save those for method signatures in service layers. Domain models should be plain classes.

- **Don't mix nullable value types carelessly:** `int?` is different from `int`. If a value is always present, use `int`, not `int?`.

---

## Self-Review Checklist

- [ ] All three entity classes created in `Domain/` folder
- [ ] `#nullable enable` at the top of each `.cs` file
- [ ] `Id` properties use `{ get; init; }` (immutable after creation)
- [ ] Name fields (`FirstName`, `LastName`) are non-nullable with `init` (required at creation)
- [ ] Optional fields (e.g., `MiddleName`, `InsuranceId`) use `?` and `{ get; set; }`
- [ ] Nullable value types (e.g., `LicenseExpiryDate?`, `RescheduledTime?`) use `?` after the type name
- [ ] At least one computed property per entity (e.g., `FullName`, `IsUpcoming()`)
- [ ] `Appointment` references `DoctorId` and `PatientId` (foreign keys, not navigation properties yet—that's EF Core)
- [ ] `Program.cs` can construct instances of all three classes without errors
- [ ] `dotnet build` completes with zero warnings and zero errors
- [ ] You can explain why each property is `init` vs `set` vs read-only
