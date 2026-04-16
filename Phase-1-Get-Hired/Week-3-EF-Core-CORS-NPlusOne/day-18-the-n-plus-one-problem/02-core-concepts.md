# CORE CONCEPT BREAKDOWN

## 1. Observability: Seeing the SQL
To solve N+1, you must first see it. In .NET, you configure the logging level in `appsettings.Development.json`.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```
Setting `Microsoft.EntityFrameworkCore.Database.Command` to `Information` prints every single SQL statement EF Core sends to the server into your debug console.

## 2. The N+1 Pattern
The N+1 problem occurs when you fetch a root entity (the "1") and then, for each of those entities, you fetch related data (the "N").

### The "Junior" Implementation (The Bug)
```csharp
// 1 Query: Fetch all doctors
var doctors = await _context.Doctors.ToListAsync(); 

foreach (var doc in doctors)
{
    // N Queries: For every doctor, EF Core fires a NEW query to fetch their appointments
    // because they weren't included in the initial fetch.
    Console.WriteLine($"Doctor {doc.Name} has {doc.Appointments.Count} appointments");
}
```
**SQL Result:**
1. `SELECT * FROM Doctors;` (The 1)
2. `SELECT * FROM Appointments WHERE DoctorId = 1;` (N)
3. `SELECT * FROM Appointments WHERE DoctorId = 2;` (N)
... (up to 100 times if you have 100 doctors)

## 3. The Fixes

### A. Eager Loading (`.Include`)
Fetch everything in one go using a `JOIN`.
```csharp
var doctors = await _context.Doctors
    .Include(d => d.Appointments) // Joins Appointments table
    .ThenInclude(a => a.Patient)    // Joins Patients table (nested)
    .ToListAsync();
```
**SQL Result:** One single, slightly larger query with multiple JOINs.

### B. Projection (`.Select`)
Only fetch the specific columns you need. This is the most performant approach.
```csharp
var doctorSummaries = await _context.Doctors
    .Select(d => new {
        d.Name,
        AppointmentCount = d.Appointments.Count // EF Core converts this to a SQL COUNT()
    })
    .ToListAsync();
```

## 4. IQueryable vs IEnumerable (The Performance Cliff)
This is the most critical distinction in EF Core.

- **`IQueryable<T>`**: A "Query Builder." It represents a SQL query that **has not been executed yet**. Any `.Where()`, `.OrderBy()`, or `.Select()` called on it is translated into SQL.
- **`IEnumerable<T>`**: A "Collection in Memory." Once you call `.ToList()`, `.ToArray()`, or `.FirstOrDefault()`, the query is executed, and the data is moved from SQL Server into your RAM.

### The Disaster Scenario
```csharp
// WRONG: .ToList() converts IQueryable to IEnumerable immediately.
var results = _context.Patients.ToList() 
    .Where(p => p.City == "Cairo"); 
```
**What happens:** EF Core executes `SELECT * FROM Patients` (downloading 1 million rows into RAM) and then uses C# to filter them.

```csharp
// RIGHT: Filter remains IQueryable.
var results = _context.Patients
    .Where(p => p.City == "Cairo")
    .ToList(); 
```
**What happens:** EF Core executes `SELECT * FROM Patients WHERE City = 'Cairo'` (downloading only the matching 100 rows).
