# QUIZ — Day 4: LINQ, IQueryable vs IEnumerable, and Interface Design

Answer all 5 questions. Paste your answers into the chat when ready.

---

## Q1 (Conceptual): IQueryable vs IEnumerable

Explain the difference between `IQueryable<Appointment>` and `IEnumerable<Appointment>`, focusing on:
1. Where filtering happens (server vs memory)
2. What SQL is generated (or not)
3. When you'd use each one
4. The performance implications of using IEnumerable on a database query

---

## Q2 (Code-Based): Deferred Execution

```csharp
IQueryable<Doctor> query = _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .Select(d => new { d.FirstName, d.LastName })
    .OrderBy(d => d.LastName);

var count = query.Count();  // Line A
var list = query.ToList();  // Line B
```

How many database queries are executed, and at which lines? Explain why.

---

## Q3 (Code-Based): N+1 Problem and Fix

```csharp
// ❌ BAD: N+1 queries
var doctors = await _db.Doctors.ToListAsync();
foreach (var doctor in doctors)
{
    var appointments = doctor.Appointments.ToList();  // Query per doctor
}
```

Rewrite this to fetch all appointments in a single query using `.Include()`, and explain why it's better.

---

## Q4 (Scenario-Based): Repository Pattern and Interface Segregation

You have a service that only reads data (no writes). Should the service depend on `IRepository<T>` (read + write) or `IReadRepository<T>` (read only)? Explain the principle and benefit.

---

## Q5 (Code-Based): Full-Table Scan

```csharp
var appointments = await _db.Appointments
    .ToList()
    .Where(a => a.DoctorId == 1)
    .ToList();
```

What problem does this code have? Rewrite it to execute the filter in SQL, and explain the performance difference.

---

**When you've answered all five, paste them into the chat and I'll grade them.**
