# CORE CONCEPTS — LINQ Execution, IEnumerable vs IQueryable, and Interface Design

## 1. LINQ Execution Models: Deferred vs Immediate

### What Is LINQ?

LINQ stands for **Language-Integrated Query**. It provides a uniform syntax for querying any data source: in-memory collections, databases, XML, etc.

```csharp
// Query syntax
var doctors = from d in _db.Doctors
              where d.Specialty == "Cardiology"
              select d;

// Method syntax (equivalent, more common in modern C#)
var doctors = _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .ToList();
```

### Deferred Execution

Most LINQ operations are **deferred**. The query is not executed until you enumerate it or materialize it.

```csharp
IEnumerable<Appointment> query = _db.Appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .OrderBy(a => a.ScheduledTime);

// At this point, NO database query has executed
// The query variable just holds the instructions

// Execution happens HERE when we iterate
foreach (var apt in query)
{
    Console.WriteLine(apt.Id);
}
```

**Key insight:** The query variable is lazy. It's a recipe, not the result.

### Immediate Execution

Certain operations **force immediate execution**:

```csharp
IEnumerable<Appointment> query = _db.Appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .ToList();  // ← Immediate execution; results in memory

// Now query is a List<Appointment>, fully materialized
```

| Operation | Execution |
|---|---|
| `.Where()`, `.Select()`, `.OrderBy()` | Deferred |
| `.ToList()`, `.ToArray()`, `.ToDictionary()` | Immediate |
| `.Count()`, `.Any()`, `.First()` | Immediate |
| `.FirstOrDefault()`, `.SingleOrDefault()` | Immediate |

### Danger: Multiple Enumeration

```csharp
IEnumerable<Appointment> query = _db.Appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled);

// First enumeration: query executes
var count = query.Count();  // ← Database query #1

// Second enumeration: query executes AGAIN
var first = query.FirstOrDefault();  // ← Database query #2
```

**You issued two database queries!** For IQueryable, this is usually OK (the query is sent to the database twice). For IEnumerable over a large dataset, this is inefficient.

**Best practice:** Materialize once if you'll enumerate multiple times.

```csharp
var appointments = _db.Appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .ToList();  // Materialize once

var count = appointments.Count;  // In-memory, no query
var first = appointments.FirstOrDefault();  // In-memory, no query
```

---

## 2. IQueryable<T> — Translation to SQL (EF Core)

### What Is IQueryable?

`IQueryable<T>` represents a **queryable data source** (typically a database). When you compose LINQ operations on an `IQueryable`, they're translated into an **expression tree**, which EF Core converts to SQL.

```csharp
IQueryable<Doctor> query = _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .OrderBy(d => d.LastName)
    .Take(10);

// The query is an expression tree, NOT executed yet
// Execution happens when you enumerate or materialize
```

### EF Core Translation Example

```csharp
var doctors = await _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .Select(d => new { d.FirstName, d.LastName })  // Projection
    .OrderBy(d => d.LastName)
    .ToListAsync();
```

EF Core translates this to:

```sql
SELECT [d].[FirstName], [d].[LastName]
FROM [Doctors] AS [d]
WHERE [d].[Specialty] = N'Cardiology'
ORDER BY [d].[LastName]
```

**Only the projected columns are fetched.** No unnecessary data transfer.

### IQueryable Composability

You can compose IQueryable operations and pass them around:

```csharp
public IQueryable<Appointment> GetScheduledAppointments()
{
    return _db.Appointments
        .Where(a => a.Status == AppointmentStatus.Scheduled);
}

public IQueryable<Appointment> FilterByDoctor(IQueryable<Appointment> query, int doctorId)
{
    return query.Where(a => a.DoctorId == doctorId);
}

// Usage
var query = GetScheduledAppointments();  // IQueryable, not executed
query = FilterByDoctor(query, doctorId: 1);  // Still IQueryable, still not executed
var results = await query.ToListAsync();  // NOW it executes, as a single SQL query
```

**All filtering happens in SQL**, not in memory. Efficient.

---

## 3. IEnumerable<T> — In-Memory Collections

### What Is IEnumerable?

`IEnumerable<T>` is the simplest iteration abstraction. It's used for **in-memory collections** or when you're working with LINQ-to-Objects (in-memory filtering/projecting).

```csharp
List<Doctor> doctors = new()
{
    new Doctor { FirstName = "Alice", Specialty = "Cardiology" },
    new Doctor { FirstName = "Bob", Specialty = "Neurology" }
};

IEnumerable<Doctor> query = doctors
    .Where(d => d.Specialty == "Cardiology")
    .Select(d => d.FirstName);

// Execution happens when you enumerate
foreach (var name in query)
{
    Console.WriteLine(name);  // "Alice"
}
```

All filtering and projecting happens **in-memory**, not in a database.

---

## 4. IQueryable vs IEnumerable: Performance Implications

### The Critical Difference

```csharp
// IQueryable: filtering on the database server
IQueryable<Appointment> query = _db.Appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled);  // WHERE in SQL

var results = await query.ToListAsync();  // Executes in SQL

// IEnumerable: filtering in memory
IEnumerable<Appointment> query = _db.Appointments.ToList()  // Fetch ALL
    .Where(a => a.Status == AppointmentStatus.Scheduled);  // Filter in memory

var results = query.ToList();  // All rows fetched, then filtered
```

**The IQueryable query uses a WHERE clause in SQL.** Only matching rows are sent.

**The IEnumerable query fetches all rows,** then filters in-memory. Disaster if the table is large.

### N+1 Problem (IQueryable)

```csharp
var doctors = _db.Doctors.ToList();  // Fetch all doctors (1 query)

foreach (var doctor in doctors)
{
    var appointments = doctor.Appointments.ToList();  // ← N queries (one per doctor)
}
```

Total queries: 1 + N. This is the **N+1 problem.**

**Fix: Use eager loading**

```csharp
var doctors = await _db.Doctors
    .Include(d => d.Appointments)  // Join loaded in a single query
    .ToListAsync();

foreach (var doctor in doctors)
{
    var appointments = doctor.Appointments;  // In-memory, no query
}
```

Total queries: 1.

### Full-Table Scan (IQueryable)

```csharp
// ❌ Bad: all appointments fetched, then filtered in memory
var appointments = _db.Appointments
    .ToList()  // ← Fetch ALL rows
    .Where(a => a.DoctorId == 1)  // ← Filter in memory
    .ToList();

// ✅ Good: filtering in SQL
var appointments = await _db.Appointments
    .Where(a => a.DoctorId == 1)  // ← WHERE in SQL
    .ToListAsync();
```

The bad version is a full-table scan. The good version uses an index.

---

## 5. Projection and DTO Pattern

### Projection Reduces Data Transfer

```csharp
// ❌ Fetch entire Doctor entity
var doctors = await _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .ToListAsync();  // Fetches all columns for each doctor

// ✅ Project to DTO: fetch only needed columns
var doctors = await _db.Doctors
    .Where(d => d.Specialty == "Cardiology")
    .Select(d => new DoctorListDto
    {
        Id = d.Id,
        FirstName = d.FirstName,
        LastName = d.LastName,
        Specialty = d.Specialty
    })
    .ToListAsync();  // Fetches only these 4 columns
```

The projection is translated to SQL:

```sql
SELECT [d].[Id], [d].[FirstName], [d].[LastName], [d].[Specialty]
FROM [Doctors] AS [d]
WHERE [d].[Specialty] = N'Cardiology'
```

Much smaller data transfer.

---

## 6. Interface Design: Composition Over Inheritance

### The Repository Pattern

```csharp
// Read operations
public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

// Write operations
public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
```

Services can depend on `IReadRepository<T>` when they only read, and `IRepository<T>` when they read and write. This is **interface segregation**: clients depend only on what they need.

### Implementation

```csharp
public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;

    public EfRepository(DbContext context) => _context = context;

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### Service Composition

```csharp
public class AppointmentService
{
    private readonly IRepository<Appointment> _appointmentRepo;
    private readonly IReadRepository<Doctor> _doctorRepo;

    public AppointmentService(
        IRepository<Appointment> appointmentRepo,
        IReadRepository<Doctor> doctorRepo)
    {
        _appointmentRepo = appointmentRepo;
        _doctorRepo = doctorRepo;
    }

    public async Task<Appointment> ScheduleAsync(
        int doctorId,
        int patientId,
        DateTime scheduledTime,
        CancellationToken cancellationToken = default)
    {
        // Check doctor exists
        var doctor = await _doctorRepo.GetByIdAsync(doctorId, cancellationToken);
        if (doctor is null)
            throw new InvalidOperationException("Doctor not found");

        // Create and save appointment
        var apt = new Appointment { DoctorId = doctorId, PatientId = patientId, ScheduledTime = scheduledTime };
        await _appointmentRepo.AddAsync(apt, cancellationToken);

        return apt;
    }
}
```

**Benefits:**
- **Testable:** Mock `IRepository<T>` for tests.
- **Flexible:** Swap implementations (EF, Dapper, mock) without changing services.
- **Decoupled:** Service doesn't know or care how data is persisted.

---

## 7. Default Interface Implementations (C# 8+)

Default methods allow interfaces to evolve without breaking implementers:

```csharp
public interface IRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Default implementation (new feature)
    async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            await AddAsync(entity, cancellationToken);
    }
}

// Existing implementations automatically get AddRangeAsync without code change
public class EfRepository<T> : IRepository<T> where T : class
{
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Existing implementation
    }
    // AddRangeAsync inherited from interface default
}
```

**Caution:** Default interface methods are rarely used in production. They're powerful for library evolution but can make code harder to understand. Use sparingly and document explicitly.

---

## ASCII Diagram: LINQ Execution Flow

```
Compose IQueryable chain
         │
    Where(x => x.Status == "Scheduled")
         │
    Select(x => new { x.Id, x.DoctorId })
         │
    OrderBy(x => x.Id)
         │
    ▼ (No execution yet)
    
    │
    └─→ ToListAsync() or FirstOrDefaultAsync()
         │
         ▼ (Execution triggered)
    
    Expression tree built
         │
         ▼
    EF Core translates to SQL
         │
         ▼
    SQL sent to database
         │
         ▼
    Results returned and materialized
         │
         ▼
    Results in memory
```

---

## Summary Table: LINQ at a Glance

| Operation | Type | Execution | Use |
|---|---|---|---|
| `.Where(predicate)` | Deferred | On enumeration | Filtering |
| `.Select(projection)` | Deferred | On enumeration | Transformation |
| `.OrderBy(keySelector)` | Deferred | On enumeration | Sorting |
| `.Take(n)`, `.Skip(n)` | Deferred | On enumeration | Pagination |
| `.ToList()` | Immediate | Now | Materialize to memory |
| `.FirstOrDefault()` | Immediate | Now | Get first or null |
| `.Count()` | Immediate | Now | Count rows |
| `.Include(navigation)` | IQueryable only | On execution | Eager load relations |
| `.Where().ToList()` | IEnumerable | On enumeration | In-memory filter |
| `.ToList().Where()` | In-memory only | On enumeration | Full load, then filter |
