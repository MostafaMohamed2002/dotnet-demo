# Day 6: First Real Code – HTTP Client & LINQ Apps – Integration Task (Optional)

## Project: Combined Clinic Management System

Combine App 1 (HTTP client) and App 2 (LINQ engine) to build a simple clinic system that:
1. Fetches appointments from an API
2. Caches them in memory
3. Queries the cache with LINQ

**Time**: 30–45 minutes (optional)

---

## Part 1: Create the Project

```bash
$ dotnet new console --name ClinicManagementSystem
$ cd ClinicManagementSystem
```

---

## Part 2: Integrate HTTP Client + LINQ

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// ============ Models ============

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = "";
    public string DoctorName { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = "";
    public AppointmentStatus Status { get; set; }
}

public enum AppointmentStatus { Scheduled, Completed, Cancelled }

// ============ Clinic API Client ============

public class ClinicApiClient
{
    private readonly HttpClient _httpClient;

    public ClinicApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
        };
    }

    public async Task<List<Appointment>> FetchAppointmentsAsync(
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var appointments = new List<Appointment>();

        for (int i = 1; i <= count; i++)
        {
            try
            {
                var response = await _httpClient.GetAsync($"todos/{i}", cancellationToken);
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                appointments.Add(new Appointment
                {
                    Id = i,
                    PatientName = $"Patient-{root.GetProperty("userId").GetInt32()}",
                    DoctorName = "Dr. Smith",
                    AppointmentDate = DateTime.Now.AddDays(i),
                    Reason = root.GetProperty("title").GetString() ?? "Checkup",
                    Status = root.GetProperty("completed").GetBoolean()
                        ? AppointmentStatus.Completed
                        : AppointmentStatus.Scheduled
                });
            }
            catch { /* Skip on error */ }
        }

        return appointments;
    }
}

// ============ Clinic Query Engine ============

public class ClinicQueryEngine
{
    private readonly List<Appointment> _cache;

    public ClinicQueryEngine(List<Appointment> appointments)
    {
        _cache = appointments;
    }

    public List<Appointment> GetScheduledAppointments()
        => _cache.Where(a => a.Status == AppointmentStatus.Scheduled).ToList();

    public List<Appointment> GetAppointmentsByDoctor(string doctorName)
        => _cache.Where(a => a.DoctorName == doctorName).ToList();

    public List<Appointment> GetUpcomingAppointments(int daysAhead = 7)
    {
        var threshold = DateTime.Now.AddDays(daysAhead);
        return _cache
            .Where(a => a.AppointmentDate <= threshold && a.Status == AppointmentStatus.Scheduled)
            .OrderBy(a => a.AppointmentDate)
            .ToList();
    }

    public Dictionary<string, int> GetAppointmentCountByDoctor()
        => _cache
            .GroupBy(a => a.DoctorName)
            .ToDictionary(g => g.Key, g => g.Count());

    public int GetTotalAppointments() => _cache.Count;
    public int GetCompletedAppointments() => _cache.Count(a => a.Status == AppointmentStatus.Completed);
}

// ============ Main Program ============

Console.WriteLine("=== Clinic Management System ===\n");

// Step 1: Fetch appointments from API
Console.WriteLine("Step 1: Fetching appointments from API...");
var apiClient = new ClinicApiClient();
var appointments = await apiClient.FetchAppointmentsAsync(count: 5);
Console.WriteLine($"Fetched {appointments.Count} appointments\n");

// Step 2: Create query engine with cached data
Console.WriteLine("Step 2: Building query engine from cached data...");
var queryEngine = new ClinicQueryEngine(appointments);
Console.WriteLine("Query engine ready\n");

// Step 3: Run various queries
Console.WriteLine("Step 3: Running queries...\n");

// Query 1: Scheduled appointments
var scheduled = queryEngine.GetScheduledAppointments();
Console.WriteLine($"Scheduled appointments: {scheduled.Count}");
foreach (var a in scheduled.Take(3))
    Console.WriteLine($"  - {a.PatientName} ({a.Status})");
Console.WriteLine();

// Query 2: Dr. Smith appointments
var smithAppointments = queryEngine.GetAppointmentsByDoctor("Dr. Smith");
Console.WriteLine($"Dr. Smith has {smithAppointments.Count} appointments\n");

// Query 3: Upcoming appointments
var upcoming = queryEngine.GetUpcomingAppointments(daysAhead: 10);
Console.WriteLine($"Upcoming appointments (next 10 days): {upcoming.Count}");
foreach (var a in upcoming)
    Console.WriteLine($"  - {a.PatientName} on {a.AppointmentDate:yyyy-MM-dd}");
Console.WriteLine();

// Query 4: Statistics
var countByDoctor = queryEngine.GetAppointmentCountByDoctor();
Console.WriteLine("Statistics:");
Console.WriteLine($"  Total appointments: {queryEngine.GetTotalAppointments()}");
Console.WriteLine($"  Completed: {queryEngine.GetCompletedAppointments()}");
Console.WriteLine($"  Doctors: {string.Join(", ", countByDoctor.Keys)}\n");

// Step 4: Advanced query (direct LINQ)
Console.WriteLine("Step 4: Advanced direct LINQ query...");
var report = appointments
    .GroupBy(a => a.Status)
    .OrderByDescending(g => g.Count())
    .Select(g => new { Status = g.Key, Count = g.Count() })
    .ToList();

Console.WriteLine("Appointment status report:");
foreach (var row in report)
    Console.WriteLine($"  {row.Status}: {row.Count}");

Console.WriteLine("\n=== System Complete ===");
```

---

## Part 3: Build and Run

```bash
$ dotnet build
$ dotnet run
```

**Expected output shows:**
- Fetched appointments from API
- Query results (scheduled, by doctor, upcoming)
- Statistics and reports

---

## Architecture

```
┌─────────────────┐
│  ClinicApiClient│  (HTTP layer)
│  Fetches from   │
│  external API   │
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│  In-Memory List │  (Cache layer)
│  of Appointments│
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│ ClinicQueryEngine│  (Query layer)
│  LINQ queries   │
└─────────────────┘
```

**Separation of concerns:**
- API client handles HTTP
- Query engine handles LINQ
- Main orchestrates both

---

## Key Insights

1. **Layered design**: API fetch → cache → query
2. **In-memory LINQ**: Fast queries on cached data
3. **Reusability**: Query engine methods encapsulate logic
4. **Extensibility**: Easy to add new queries

---

## Enhancement: Add Persistence

```csharp
// Save to file after fetch
System.IO.File.WriteAllText("appointments.json", 
    JsonSerializer.Serialize(appointments));

// Load from file instead of fetching
var cached = JsonSerializer.Deserialize<List<Appointment>>(
    System.IO.File.ReadAllText("appointments.json"));
```

---

## Summary

You now have three complete, runnable apps:
1. **App 1 (HttpClient)**: Fetches from API
2. **App 2 (LINQ)**: Queries in-memory
3. **App 3 (Integration)**: Combines both

All use real patterns you'll see in production backend systems.

Next: Day 6 Quiz and Summary.
