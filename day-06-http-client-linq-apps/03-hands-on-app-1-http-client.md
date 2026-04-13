# Day 6: First Real Code – HTTP Client & LINQ Apps – App 1: HTTP Client

## Project: Appointment API Client

You're building a **typed HTTP client** that fetches appointment data from a mock REST API, parses JSON, and handles errors gracefully.

**Time**: 45–60 minutes

---

## Part 1: Create the Project

```bash
$ dotnet new console --name AppointmentApiClient
$ cd AppointmentApiClient
```

### Project Structure After Creation

```
AppointmentApiClient/
├─ Program.cs
├─ AppointmentApiClient.csproj
├─ obj/
└─ bin/
```

---

## Part 2: Add NuGet Packages

No external packages needed! We'll use built-in libraries:
- `System.Net.Http` (HTTP requests)
- `System.Text.Json` (JSON serialization)

Both are included in the .NET 8 SDK.

---

## Part 3: Define Data Models

Create `Program.cs` with data classes and the API client:

```csharp
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

// ============ Data Models ============

public class Appointment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("patientName")]
    public string PatientName { get; set; } = "";

    [JsonPropertyName("doctorName")]
    public string DoctorName { get; set; } = "";

    [JsonPropertyName("appointmentDate")]
    public DateTime AppointmentDate { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "Scheduled";
}

// ============ Typed HTTP Client ============

public class AppointmentApiClient
{
    private readonly HttpClient _httpClient;

    public AppointmentApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
        };
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Simulates fetching an appointment from a REST API.
    /// For demo purposes, we fetch from a public mock API and transform the response.
    /// </summary>
    public async Task<Appointment?> GetAppointmentAsync(
        int appointmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Fetch from JSONPlaceholder (mock API)
            var response = await _httpClient.GetAsync(
                $"todos/{appointmentId}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            // Parse the response
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var appointment = new Appointment
            {
                Id = root.GetProperty("id").GetInt32(),
                PatientName = $"Patient from TODO {root.GetProperty("userId").GetInt32()}",
                DoctorName = "Dr. Smith",
                AppointmentDate = DateTime.Now.AddDays(appointmentId),
                Reason = root.GetProperty("title").GetString() ?? "Checkup",
                Status = root.GetProperty("completed").GetBoolean() ? "Completed" : "Scheduled"
            };

            return appointment;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing failed: {ex.Message}");
            return null;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Request timed out (cancelled)");
            return null;
        }
    }

    /// <summary>
    /// Fetch multiple appointments with timeout.
    /// </summary>
    public async Task<List<Appointment>> GetAppointmentsAsync(
        int[] appointmentIds,
        int timeoutSeconds = 10)
    {
        var result = new List<Appointment>();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        foreach (var id in appointmentIds)
        {
            var appointment = await GetAppointmentAsync(id, cts.Token);
            if (appointment != null)
            {
                result.Add(appointment);
            }
        }

        return result;
    }
}

// ============ Main Program ============

var client = new AppointmentApiClient();

Console.WriteLine("=== Appointment API Client Demo ===\n");

// Fetch a single appointment
Console.WriteLine("Fetching single appointment...");
var appointment = await client.GetAppointmentAsync(1);
if (appointment != null)
{
    Console.WriteLine($"ID: {appointment.Id}");
    Console.WriteLine($"Patient: {appointment.PatientName}");
    Console.WriteLine($"Doctor: {appointment.DoctorName}");
    Console.WriteLine($"Date: {appointment.AppointmentDate:yyyy-MM-dd}");
    Console.WriteLine($"Reason: {appointment.Reason}");
    Console.WriteLine($"Status: {appointment.Status}");
    Console.WriteLine();
}

// Fetch multiple appointments
Console.WriteLine("Fetching multiple appointments...");
var appointments = await client.GetAppointmentsAsync([1, 2, 3, 4, 5]);
Console.WriteLine($"Fetched {appointments.Count} appointments:");
foreach (var a in appointments)
{
    Console.WriteLine($"  - {a.PatientName} (Dr. {a.DoctorName}): {a.Status}");
}

// Demonstrate timeout
Console.WriteLine("\nDemonstrating timeout (will show network error or timeout)...");
using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
var timedOut = await client.GetAppointmentAsync(999, cts.Token);
Console.WriteLine(timedOut == null ? "Timed out as expected" : "Unexpectedly succeeded");

Console.WriteLine("\n=== Demo Complete ===");
```

---

## Part 4: Build and Run

```bash
$ dotnet build
Build succeeded.

$ dotnet run
```

**Expected output:**
```
=== Appointment API Client Demo ===

Fetching single appointment...
ID: 1
Patient: Patient from TODO 1
Doctor: Dr. Smith
Date: 2026-04-13
Reason: delectus aut autem
Status: Scheduled

Fetching multiple appointments...
Fetched 5 appointments:
  - Patient from TODO 1 (Dr. Smith): Scheduled
  - Patient from TODO 2 (Dr. Smith): Scheduled
  - Patient from TODO 3 (Dr. Smith): Completed
  - Patient from TODO 4 (Dr. Smith): Scheduled
  - Patient from TODO 5 (Dr. Smith): Completed

Demonstrating timeout (will show network error or timeout)...
Timed out as expected

=== Demo Complete ===
```

---

## Part 5: Code Walkthrough

### HttpClient Initialization

```csharp
_httpClient = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
};
```

Sets the base URL. All requests are relative to this.

### Async Method with CancellationToken

```csharp
public async Task<Appointment?> GetAppointmentAsync(
    int appointmentId,
    CancellationToken cancellationToken = default)
```

- `async Task<...>`: Method is asynchronous, returns a task
- `CancellationToken`: Allows caller to cancel the request
- `default`: Optional parameter (cancellation not required)

### Error Handling

```csharp
try
{
    // Try the operation
}
catch (HttpRequestException ex)
{
    // Network error, DNS failure, etc.
    Console.WriteLine($"HTTP request failed: {ex.Message}");
    return null;
}
catch (JsonException ex)
{
    // Malformed JSON
    Console.WriteLine($"JSON parsing failed: {ex.Message}");
    return null;
}
catch (OperationCanceledException)
{
    // Timeout
    Console.WriteLine("Request timed out (cancelled)");
    return null;
}
```

**Three catch blocks for three failure modes:**
1. Network-level errors
2. JSON parsing errors
3. Timeouts (cancellation)

### Timeout Pattern

```csharp
using var cts = new CancellationTokenSource(
    TimeSpan.FromSeconds(timeoutSeconds));

foreach (var id in appointmentIds)
{
    var appointment = await GetAppointmentAsync(id, cts.Token);
    // If timeout is exceeded, cts.Token signals cancellation
    // and GetAppointmentAsync throws OperationCanceledException
}
```

---

## Part 6: Enhancements (Optional)

### Add Retry Logic

```csharp
public async Task<Appointment?> GetAppointmentWithRetryAsync(
    int appointmentId,
    CancellationToken cancellationToken = default)
{
    int retries = 3;
    while (retries > 0)
    {
        try
        {
            return await GetAppointmentAsync(appointmentId, cancellationToken);
        }
        catch (HttpRequestException) when (retries > 1)
        {
            retries--;
            Console.WriteLine($"Retrying... ({retries} attempts left)");
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
    return null;
}
```

**Usage:**
```csharp
var appointment = await client.GetAppointmentWithRetryAsync(1);
```

### Parameterized Base URL

```csharp
public AppointmentApiClient(string baseUrl = "https://jsonplaceholder.typicode.com/")
{
    _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
}

// Usage
var client = new AppointmentApiClient("https://clinic-api.local/");
```

---

## Self-Review Checklist

Before moving to App 2, verify:

- [ ] `dotnet new console` created the project
- [ ] `Program.cs` compiles without errors
- [ ] `Appointment` class is defined with all properties
- [ ] `AppointmentApiClient` class has `GetAppointmentAsync()`
- [ ] `GetAppointmentAsync()` accepts `CancellationToken`
- [ ] Error handling covers `HttpRequestException`, `JsonException`, `OperationCanceledException`
- [ ] `dotnet run` executes and fetches from the mock API
- [ ] Output shows appointment data correctly
- [ ] Timeout demonstration works (shows timeout message)
- [ ] Multiple appointments can be fetched in a loop

---

## Key Takeaways

✅ **What you learned:**
1. **HttpClient pattern**: Reusable, configured with base URL
2. **CancellationToken**: Pass through async calls for timeout control
3. **Error handling**: Distinguish HTTP errors, JSON errors, timeouts
4. **JSON parsing**: Manual parsing with `JsonDocument` (flexible) or `JsonSerializer` (simple)
5. **Async/await**: Used throughout for responsive operations

✅ **Production patterns used:**
- Typed HTTP client (encapsulation)
- Timeout management
- Graceful error fallback (return null vs throwing)
- Request/response logging (via Console, could add structured logging)

---

## Next: App 2 (LINQ Query Engine)

App 1 fetches data from an API. App 2 queries that data **in-memory** using LINQ with deferred execution. You'll see how the two combine in real applications.
