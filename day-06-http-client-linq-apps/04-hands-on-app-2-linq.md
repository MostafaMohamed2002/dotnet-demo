# Day 6: First Real Code – HTTP Client & LINQ Apps – App 2: LINQ Query Engine

## Project: Appointment Query Engine

You're building an **in-memory LINQ query engine** that demonstrates deferred execution, materialization, and optimization patterns.

**Time**: 45–60 minutes

---

## Part 1: Create the Project

```bash
$ dotnet new console --name AppointmentQueryEngine
$ cd AppointmentQueryEngine
```

---

## Part 2: Define Data Model & Sample Data

Create `Program.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

// ============ Data Model ============

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = "";
    public AppointmentStatus Status { get; set; }
}

public enum AppointmentStatus
{
    Scheduled = 0,
    Completed = 1,
    Cancelled = 2
}

// ============ Sample Data ============

var appointments = new List<Appointment>
{
    new()
    {
        Id = 101,
        PatientName = "Alice Johnson",
        DoctorName = "Dr. Smith",
        AppointmentDate = DateTime.Parse("2026-04-15 10:30"),
        Reason = "Checkup",
        Status = AppointmentStatus.Scheduled
    },
    new()
    {
        Id = 102,
        PatientName = "Bob Williams",
        DoctorName = "Dr. Jones",
        AppointmentDate = DateTime.Parse("2026-04-14 14:00"),
        Reason = "Follow-up",
        Status = AppointmentStatus.Scheduled
    },
    new()
    {
        Id = 103,
        PatientName = "Charlie Brown",
        DoctorName = "Dr. Smith",
        AppointmentDate = DateTime.Parse("2026-04-10 09:00"),
        Reason = "Surgery",
        Status = AppointmentStatus.Completed
    },
    new()
    {
        Id = 104,
        PatientName = "Diana Prince",
        DoctorName = "Dr. Wilson",
        AppointmentDate = DateTime.Parse("2026-04-20 16:30"),
        Reason = "Consultation",
        Status = AppointmentStatus.Scheduled
    },
    new()
    {
        Id = 105,
        PatientName = "Eve Martinez",
        DoctorName = "Dr. Smith",
        AppointmentDate = DateTime.Parse("2026-04-12 11:00"),
        Reason = "Checkup",
        Status = AppointmentStatus.Cancelled
    },
    new()
    {
        Id = 106,
        PatientName = "Frank Thompson",
        DoctorName = "Dr. Jones",
        AppointmentDate = DateTime.Parse("2026-04-18 13:30"),
        Reason = "Lab work",
        Status = AppointmentStatus.Scheduled
    }
};

Console.WriteLine("=== Appointment Query Engine Demo ===\n");

// ============ Demo 1: Deferred Execution ============

Console.WriteLine("--- Demo 1: Deferred Execution ---");
Console.WriteLine("Defining query (not running yet)...");

IEnumerable<Appointment> scheduledQuery = appointments
    .Where(a =>
    {
        Console.WriteLine($"  [Where] Checking {a.PatientName}");
        return a.Status == AppointmentStatus.Scheduled;
    });

Console.WriteLine("Query defined. Nothing printed above (deferred execution).\n");

Console.WriteLine("Iterating through results (now the query runs):");
foreach (var a in scheduledQuery)
{
    Console.WriteLine($"  {a.PatientName} with Dr. {a.DoctorName}");
}
Console.WriteLine();

// ============ Demo 2: Multiple Enumeration ============

Console.WriteLine("--- Demo 2: Multiple Enumeration (Query Runs Twice) ---");

var reusableQuery = appointments
    .Where(a =>
    {
        Console.WriteLine($"  [Where] Checking {a.PatientName}");
        return a.Status == AppointmentStatus.Scheduled;
    });

Console.WriteLine("First iteration:");
var count = reusableQuery.Count();  // 1st enumeration
Console.WriteLine($"Scheduled appointments: {count}\n");

Console.WriteLine("Second iteration:");
var first = reusableQuery.FirstOrDefault();  // 2nd enumeration
Console.WriteLine($"First scheduled: {first?.PatientName}\n");

Console.WriteLine("Notice: [Where] was called 6 times total (3 patients × 2 iterations)");
Console.WriteLine("The query ran twice because IEnumerable is lazy.\n");

// ============ Demo 3: Materialization with ToList() ============

Console.WriteLine("--- Demo 3: Materialization (Single Pass) ---");

var materializedQuery = appointments
    .Where(a =>
    {
        Console.WriteLine($"  [Where] Checking {a.PatientName}");
        return a.Status == AppointmentStatus.Scheduled;
    })
    .ToList();  // Runs the query here, stores result

Console.WriteLine($"Materialized {materializedQuery.Count} appointments\n");

Console.WriteLine("First iteration (no additional Where calls):");
foreach (var a in materializedQuery)
{
    Console.WriteLine($"  {a.PatientName}");
}
Console.WriteLine();

Console.WriteLine("Second iteration (still no additional Where calls):");
foreach (var a in materializedQuery)
{
    Console.WriteLine($"  {a.PatientName}");
}
Console.WriteLine();

// ============ Demo 4: Complex Queries ============

Console.WriteLine("--- Demo 4: Complex Query Chains ---");

var upcomingBySmith = appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .Where(a => a.DoctorName == "Dr. Smith")
    .OrderBy(a => a.AppointmentDate)
    .Select(a => new { a.PatientName, a.AppointmentDate })
    .ToList();

Console.WriteLine("Upcoming appointments with Dr. Smith (sorted by date):");
foreach (var a in upcomingBySmith)
{
    Console.WriteLine($"  {a.PatientName} on {a.AppointmentDate:yyyy-MM-dd HH:mm}");
}
Console.WriteLine();

// ============ Demo 5: Projection (Select) ============

Console.WriteLine("--- Demo 5: Projection (Transform Data) ---");

var patientNames = appointments
    .Where(a => a.Status == AppointmentStatus.Completed)
    .Select(a => a.PatientName)
    .ToList();

Console.WriteLine("Patients with completed appointments:");
foreach (var name in patientNames)
{
    Console.WriteLine($"  - {name}");
}
Console.WriteLine();

// ============ Demo 6: Grouping ============

Console.WriteLine("--- Demo 6: Grouping (Group by Doctor) ---");

var byDoctor = appointments
    .GroupBy(a => a.DoctorName)
    .OrderByDescending(g => g.Count())
    .ToList();

foreach (var doctorGroup in byDoctor)
{
    Console.WriteLine($"Dr. {doctorGroup.Key}: {doctorGroup.Count()} appointments");
    foreach (var a in doctorGroup)
    {
        Console.WriteLine($"  - {a.PatientName} ({a.Status})");
    }
}
Console.WriteLine();

// ============ Demo 7: Aggregation ============

Console.WriteLine("--- Demo 7: Aggregation (Count, First, Any) ---");

var totalAppointments = appointments.Count();
var completedCount = appointments.Count(a => a.Status == AppointmentStatus.Completed);
var firstScheduled = appointments.FirstOrDefault(a => a.Status == AppointmentStatus.Scheduled);
var hasSmithAppointments = appointments.Any(a => a.DoctorName == "Dr. Smith");

Console.WriteLine($"Total appointments: {totalAppointments}");
Console.WriteLine($"Completed: {completedCount}");
Console.WriteLine($"First scheduled: {firstScheduled?.PatientName}");
Console.WriteLine($"Has Dr. Smith appointments: {hasSmithAppointments}");
Console.WriteLine();

// ============ Demo 8: Optimization - Avoid Early Materialization ============

Console.WriteLine("--- Demo 8: Performance - Materialization Impact ---");

Console.WriteLine("Inefficient (materialize early):");
var allAppointments = appointments.ToList();  // 6 items loaded into memory
var smithAppointments = allAppointments.Where(a => a.DoctorName == "Dr. Smith").ToList();
Console.WriteLine($"Loaded {allAppointments.Count} appointments, then filtered to {smithAppointments.Count}");
Console.WriteLine();

Console.WriteLine("Efficient (filter first, materialize last):");
var efficientSmithAppointments = appointments
    .Where(a => a.DoctorName == "Dr. Smith")
    .ToList();
Console.WriteLine($"Filtered to {efficientSmithAppointments.Count} appointments directly");
Console.WriteLine();

// ============ Demo 9: Take/Skip (Pagination) ============

Console.WriteLine("--- Demo 9: Pagination (Skip & Take) ---");

var pageSize = 2;
for (int page = 1; page <= 3; page++)
{
    var pageAppointments = appointments
        .OrderBy(a => a.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    if (pageAppointments.Count == 0) break;

    Console.WriteLine($"Page {page}:");
    foreach (var a in pageAppointments)
    {
        Console.WriteLine($"  - {a.PatientName}");
    }
}
Console.WriteLine();

// ============ Demo 10: Complex Real-World Query ============

Console.WriteLine("--- Demo 10: Complex Real-World Query ---");

var reportData = appointments
    .Where(a => a.Status != AppointmentStatus.Cancelled)  // Exclude cancelled
    .GroupBy(a => a.DoctorName)
    .Select(g => new
    {
        Doctor = g.Key,
        TotalAppointments = g.Count(),
        CompletedAppointments = g.Count(a => a.Status == AppointmentStatus.Completed),
        UpcomingAppointments = g.Count(a => a.Status == AppointmentStatus.Scheduled),
        Patients = g.Select(a => a.PatientName).Distinct().Count()
    })
    .OrderByDescending(x => x.TotalAppointments)
    .ToList();

Console.WriteLine("Appointment Report by Doctor:");
foreach (var row in reportData)
{
    Console.WriteLine($"\n{row.Doctor}");
    Console.WriteLine($"  Total appointments: {row.TotalAppointments}");
    Console.WriteLine($"    - Completed: {row.CompletedAppointments}");
    Console.WriteLine($"    - Upcoming: {row.UpcomingAppointments}");
    Console.WriteLine($"  Unique patients: {row.Patients}");
}

Console.WriteLine("\n=== Demo Complete ===");
```

---

## Part 3: Build and Run

```bash
$ dotnet build
Build succeeded.

$ dotnet run
```

**Expected output:**
```
=== Appointment Query Engine Demo ===

--- Demo 1: Deferred Execution ---
Defining query (not running yet)...
Query defined. Nothing printed above (deferred execution).

Iterating through results (now the query runs):
  [Where] Checking Alice Johnson
  [Where] Checking Bob Williams
  [Where] Checking Charlie Brown
  [Where] Checking Diana Prince
  [Where] Checking Eve Martinez
  [Where] Checking Frank Thompson
  Alice Johnson with Dr. Smith
  Bob Williams with Dr. Jones
  Diana Prince with Dr. Wilson
  Frank Thompson with Dr. Jones

--- Demo 2: Multiple Enumeration (Query Runs Twice) ---
First iteration:
  [Where] Checking Alice Johnson
  [Where] Checking Bob Williams
  [Where] Checking Charlie Brown
  [Where] Checking Diana Prince
  [Where] Checking Eve Martinez
  [Where] Checking Frank Thompson
Scheduled appointments: 4

Second iteration:
  [Where] Checking Alice Johnson
  [Where] Checking Bob Williams
  [Where] Checking Charlie Brown
  [Where] Checking Diana Prince
  [Where] Checking Eve Martinez
  [Where] Checking Frank Thompson
First scheduled: Alice Johnson

Notice: [Where] was called 6 times total (3 patients × 2 iterations)
The query ran twice because IEnumerable is lazy.

... (more demos follow)

=== Demo Complete ===
```

---

## Part 4: Key Insights from Demos

### Demo 1–2: Deferred Execution

```csharp
var query = appointments.Where(...);  // Not executed yet
foreach (var a in query) { }           // Executed here
```

### Demo 3: Materialization

```csharp
var query = appointments.Where(...).ToList();  // Executed immediately
var count = query.Count();  // Uses cached list, no re-execution
```

**When to materialize**:
- Before multiple iterations (avoids re-running query)
- Before passing to methods that iterate
- When performance matters

### Demo 4–5: Chaining & Projection

```csharp
appointments
    .Where(...)     // Filter
    .OrderBy(...)   // Sort
    .Select(...)    // Transform
    .ToList();      // Materialize
```

Each operator is applied in sequence. `.Select()` transforms the shape.

### Demo 6: Grouping

```csharp
var groups = appointments.GroupBy(a => a.DoctorName);
// Returns: IEnumerable<IGrouping<string, Appointment>>
// Each group has a Key (doctor name) and items (appointments)
```

### Demo 7: Aggregation

Operators like `.Count()`, `.FirstOrDefault()`, `.Any()` materialize and return a single value.

```csharp
var count = appointments.Count(a => a.Status == AppointmentStatus.Completed);
// Counts filtered items; returns int
```

### Demo 8: Optimization

```csharp
// INEFFICIENT: Materialize all, then filter
var all = appointments.ToList();
var filtered = all.Where(...).ToList();  // Wasted memory

// EFFICIENT: Filter, then materialize
var filtered = appointments.Where(...).ToList();  // Only needed items
```

### Demo 9: Pagination

```csharp
var page = appointments
    .OrderBy(a => a.Id)
    .Skip(10)        // Skip first 10
    .Take(5)         // Take next 5
    .ToList();       // Returns items 11–15
```

Common for APIs returning paginated results.

### Demo 10: Complex Real-World

Grouping, aggregation, and transformation combined. This is typical reporting/analytics query.

---

## Part 5: Enhancements (Optional)

### Add Extension Method for Pagination

```csharp
public static class PaginationExtensions
{
    public static (List<T> Items, int TotalCount) GetPage<T>(
        this IEnumerable<T> source,
        int pageNumber,
        int pageSize)
    {
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalCount = source.Count();

        return (items, totalCount);
    }
}

// Usage
var (items, total) = appointments.GetPage(pageNumber: 2, pageSize: 3);
```

### Add Query Builder Pattern

```csharp
public class AppointmentQuery
{
    private IEnumerable<Appointment> _source;

    public AppointmentQuery(IEnumerable<Appointment> source)
    {
        _source = source;
    }

    public AppointmentQuery ByStatus(AppointmentStatus status)
    {
        _source = _source.Where(a => a.Status == status);
        return this;
    }

    public AppointmentQuery ByDoctor(string doctorName)
    {
        _source = _source.Where(a => a.DoctorName == doctorName);
        return this;
    }

    public AppointmentQuery OrderByDate()
    {
        _source = _source.OrderBy(a => a.AppointmentDate);
        return this;
    }

    public List<Appointment> ToList()
    {
        return _source.ToList();
    }
}

// Usage (fluent)
var results = new AppointmentQuery(appointments)
    .ByStatus(AppointmentStatus.Scheduled)
    .ByDoctor("Dr. Smith")
    .OrderByDate()
    .ToList();
```

---

## Self-Review Checklist

- [ ] `Appointment` class and `AppointmentStatus` enum defined
- [ ] Sample data initialized with 6 appointments
- [ ] Demo 1: Shows deferred execution (no output during query definition)
- [ ] Demo 2: Shows multiple enumeration causes double-execution (Where called 6 times)
- [ ] Demo 3: Shows `.ToList()` materializes and prevents re-execution
- [ ] Demo 4: Complex chained query works and produces correct results
- [ ] Demo 5: Projection (`.Select()`) transforms data correctly
- [ ] Demo 6: Grouping works and counts per group are correct
- [ ] Demo 7: Aggregation operators (`.Count()`, `.First()`, `.Any()`) work
- [ ] Demo 8: Shows performance difference (early vs late materialization)
- [ ] Demo 9: Pagination with `.Skip()` and `.Take()` works
- [ ] Demo 10: Complex real-world query produces a report
- [ ] `dotnet run` executes without errors
- [ ] Output is readable and demonstrates all concepts

---

## Key Takeaways

✅ **What you learned:**
1. **Deferred execution**: LINQ queries don't run until enumeration
2. **Materialization**: `.ToList()` forces execution and caches results
3. **Multiple enumeration**: Running query multiple times re-executes it (bad for performance)
4. **Chaining**: Operators compose (filter → sort → project)
5. **Grouping & aggregation**: Advanced queries combining multiple operations
6. **Optimization**: Filter before materializing, not after

✅ **Production patterns used:**
- Query builders for complex conditions
- Pagination for large datasets
- Reporting queries with grouping
- Distinct/deduplication
- Complex projections

---

## Next: Integration (Optional App 3)

You can combine App 1 (HTTP client) and App 2 (LINQ) to fetch appointments from an API and query them in-memory. This is covered in part 05-integration-task.md.
