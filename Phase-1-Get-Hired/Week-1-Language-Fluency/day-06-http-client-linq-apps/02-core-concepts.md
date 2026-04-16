# Day 6: First Real Code – HTTP Client & LINQ Apps – Core Concepts

## Part 1: System.Net.Http & HttpClient

### The Wrong Way: `new HttpClient()`

```csharp
// ❌ WRONG – Creates a new socket per request
for (int i = 0; i < 10; i++)
{
    var client = new HttpClient();  // Socket exhaustion
    var response = await client.GetAsync("https://api.example.com/data");
}
```

**Problem**: Each `new HttpClient()` opens a TCP socket. After 1,000 requests, you'll exhaust the OS socket pool. This causes:
- `SocketException`: "Unable to establish connection"
- `System.Net.Sockets.SocketException: Too many open files`
- Mysterious timeouts

### The Right Way: `IHttpClientFactory`

```csharp
// ✅ CORRECT – Reuse connection pool
public class AppointmentApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AppointmentApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Appointment> GetAppointmentAsync(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(
            $"https://clinic-api.local/appointments/{id}");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Appointment>(json);
    }
}
```

**Benefits**:
- **Connection pooling**: Reuses TCP connections (huge performance win)
- **Message handlers**: Automatically inject retry logic, logging, authentication
- **Testability**: Mock `IHttpClientFactory` in unit tests
- **Lifecycle management**: CLR manages socket cleanup

### Registering IHttpClientFactory in Dependency Injection

```csharp
// In Program.cs (Week 2 we'll do this for ASP.NET)
var services = new ServiceCollection();
services.AddHttpClient();
services.AddScoped<AppointmentApiClient>();
var provider = services.BuildServiceProvider();

// Use it
var client = provider.GetRequiredService<AppointmentApiClient>();
var appointment = await client.GetAppointmentAsync(101);
```

### Typed HTTP Clients (Encapsulation)

Instead of passing `IHttpClientFactory` everywhere, encapsulate client logic in a typed client:

```csharp
public class AppointmentApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AppointmentApiClient> _logger;

    // Constructor receives pre-configured HttpClient
    public AppointmentApiClient(HttpClient httpClient, 
        ILogger<AppointmentApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri("https://clinic-api.local/");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<Appointment> GetAppointmentAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"appointments/{id}", 
                cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Appointment>(json);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch appointment {id}", id);
            throw;  // Propagate after logging
        }
    }
}

// Register with DI
services.AddHttpClient<AppointmentApiClient>(client =>
{
    client.BaseAddress = new Uri("https://clinic-api.local/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

**Advantages**:
- Single responsibility: API client logic in one class
- Reusable: Inject into services, controllers
- Testable: Mock the typed client
- Centralized config: BaseAddress, headers in one place

---

## Part 2: System.Text.Json (Modern JSON Serialization)

### Deserializing JSON

```csharp
var json = @"
{
    ""id"": 101,
    ""patientName"": ""Alice Johnson"",
    ""doctorName"": ""Dr. Smith"",
    ""appointmentDate"": ""2026-04-15T10:30:00Z"",
    ""reason"": ""Checkup"",
    ""status"": ""Scheduled""
}";

var appointment = JsonSerializer.Deserialize<Appointment>(json);
Console.WriteLine(appointment.PatientName);  // "Alice Johnson"
```

### Configuring JsonSerializerOptions

By default, `System.Text.Json` is case-sensitive. Your C# property is `PatientName`, but JSON might have `patientName`. Fix with options:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,  // "patientName" matches "PatientName"
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // Serialize as camelCase
};

var appointment = JsonSerializer.Deserialize<Appointment>(json, options);
```

### Serializing to JSON

```csharp
var appointment = new Appointment
{
    Id = 101,
    PatientName = "Alice Johnson",
    DoctorName = "Dr. Smith",
    AppointmentDate = DateTime.Parse("2026-04-15T10:30:00"),
    Reason = "Checkup",
    Status = AppointmentStatus.Scheduled
};

var json = JsonSerializer.Serialize(appointment, options);
Console.WriteLine(json);
// Output: {"id":101,"patientName":"Alice Johnson",...}
```

### Source Generators (Compile-Time Metadata)

Instead of reflection at runtime, generate serializers at **compile time**:

```csharp
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Appointment))]
public partial class AppointmentSerializerContext : JsonSerializerContext
{
}

// At runtime (no reflection!)
var json = JsonSerializer.Serialize(
    appointment, 
    AppointmentSerializerContext.Default.Appointment);
```

**Benefits**:
- Faster (compile-time, not reflection)
- Smaller binary (no reflection overhead)
- AOT-friendly (works with ahead-of-time compilation)

---

## Part 3: CancellationToken (Timeouts & Cooperative Cancellation)

### Why Cancellation Matters

In long-running operations, you need to stop gracefully:

```csharp
// User closes the app while request is pending
// How do we stop the HTTP call without hanging?
// Answer: CancellationToken
```

### Basic Usage

```csharp
public async Task<Appointment> GetAppointmentAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    var response = await _httpClient.GetAsync(
        $"appointments/{id}",
        cancellationToken);  // Pass token through
    
    response.EnsureSuccessStatusCode();
    return await JsonSerializer.DeserializeAsync<Appointment>(
        await response.Content.ReadAsStreamAsync(cancellationToken),
        cancellationToken: cancellationToken);
}
```

### Timeout Example

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    var appointment = await GetAppointmentAsync(101, cts.Token);
    Console.WriteLine($"Got appointment: {appointment.PatientName}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out after 5 seconds");
}
```

### Propagating Cancellation

Chain tokens through your call stack:

```csharp
public async Task ProcessAppointmentsAsync(CancellationToken cancellationToken)
{
    for (int i = 1; i <= 10; i++)
    {
        var appointment = await _apiClient.GetAppointmentAsync(i, cancellationToken);
        // If cancellationToken is triggered, GetAppointmentAsync stops
        // and throws OperationCanceledException
        Console.WriteLine(appointment.PatientName);
    }
}
```

**Key rule**: Pass `CancellationToken` through **every** async method in your call chain.

---

## Part 4: LINQ & Deferred Execution

### IEnumerable vs IQueryable

| Aspect | IEnumerable | IQueryable |
|--------|-------------|-----------|
| **Execution** | Immediate (in-memory) | Deferred (to provider, e.g., DB) |
| **Materialization** | Happens on `.ToList()`, `.FirstOrDefault()`, iteration | Happens on enumeration |
| **Location** | In-memory (C# loop) | Database (SQL provider) |
| **Used for** | In-memory collections, LINQ to Objects | Entity Framework, LINQ providers |

### Deferred Execution Example

```csharp
var appointments = new List<Appointment>
{
    new() { Id = 1, PatientName = "Alice", Status = AppointmentStatus.Scheduled },
    new() { Id = 2, PatientName = "Bob", Status = AppointmentStatus.Completed },
    new() { Id = 3, PatientName = "Charlie", Status = AppointmentStatus.Scheduled },
};

// Query defined (not executed yet)
IEnumerable<Appointment> scheduled = appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled);

Console.WriteLine("Query defined");  // Prints immediately

// Iteration causes execution (NOW the Where runs)
foreach (var appointment in scheduled)
{
    Console.WriteLine(appointment.PatientName);  // "Alice", "Charlie"
}
```

**Key insight**: The `.Where()` doesn't run when defined. It runs when you iterate.

### Dangerous: Multiple Enumeration

```csharp
var scheduled = appointments.Where(a => a.Status == AppointmentStatus.Scheduled);

Console.WriteLine(scheduled.Count());     // 1st enumeration: Where runs, scans all 3 items
Console.WriteLine(scheduled.First().PatientName);  // 2nd enumeration: Where runs again

// Where is called twice! If Where has side effects, problems occur.
var doubled = appointments
    .Where(a => 
    {
        Console.WriteLine($"Checking {a.PatientName}");  // Prints per enumeration
        return a.Status == AppointmentStatus.Scheduled;
    });

foreach (var a in doubled)
    Console.WriteLine(a.PatientName);

foreach (var a in doubled)
    Console.WriteLine(a.PatientName);
// Output: "Checking" is printed 6 times (3 items × 2 iterations)
```

### Fix: Materialize with `.ToList()`

```csharp
var scheduled = appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .ToList();  // Runs WHERE immediately, stores results in memory

Console.WriteLine(scheduled.Count());  // No re-scanning
Console.WriteLine(scheduled.First().PatientName);  // Uses cached results
```

**When to materialize**:
- Before multiple iterations (`.ToList()`)
- Before passing to methods expecting `List<T>`
- When performance matters (avoid repeated filtering)

### The N+1 Problem

In SQL databases (next week, EF Core), lazy loading causes N+1:

```csharp
// Pseudo-code (EF Core behavior)
var appointments = context.Appointments.ToList();  // Query 1: SELECT * FROM Appointments (5 results)

foreach (var a in appointments)
{
    Console.WriteLine(a.Doctor.Name);  // Query 5: SELECT * FROM Doctors WHERE DoctorId = ?
    // Total: 1 + 5 = N+1 queries!
}
```

**Fix 1: Eager loading with `.Include()`**

```csharp
var appointments = context.Appointments
    .Include(a => a.Doctor)  // Join Doctor data in one query
    .ToList();

foreach (var a in appointments)
{
    Console.WriteLine(a.Doctor.Name);  // No additional queries (data already loaded)
}
```

**Fix 2: Projection (only fetch needed columns)**

```csharp
var doctorNames = context.Appointments
    .Select(a => a.Doctor.Name)  // SQL: SELECT DISTINCT DoctorName FROM Appointments
    .ToList();
// One query, no N+1
```

### LINQ Chains: Composition & Laziness

```csharp
var query = appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)    // Filter 1
    .OrderBy(a => a.AppointmentDate)                         // Sort
    .Take(10)                                                // Limit
    .Select(a => a.PatientName);                             // Project

var result = query.ToList();  // All operations run here, in sequence
```

**Execution order (conceptual)**:
1. Filter to scheduled appointments
2. Sort by date
3. Take first 10
4. Extract names
5. Return list

**Key**: The chain is lazy until `.ToList()`, then everything executes at once.

### IQueryable (Database Queries)

When you use `IQueryable<T>` (EF Core), the same chain compiles to **SQL**:

```csharp
var query = context.Appointments  // IQueryable<Appointment>
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .OrderBy(a => a.AppointmentDate)
    .Take(10)
    .Select(a => a.PatientName);

var result = query.ToList();  // Generates and executes SQL:
// SELECT TOP 10 PatientName FROM Appointments 
// WHERE Status = 0 ORDER BY AppointmentDate
```

**No in-memory iteration**. The entire chain translates to SQL. Huge performance win for large datasets.

---

## Part 5: HttpClient Error Handling

### HTTP Status Codes

```csharp
var response = await _httpClient.GetAsync("https://clinic-api.local/appointments/999");

if (response.StatusCode == System.Net.HttpStatusCode.NotFound)  // 404
{
    Console.WriteLine("Appointment not found");
}
else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)  // 401
{
    Console.WriteLine("Invalid credentials");
}
else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)  // 500
{
    Console.WriteLine("Server error");
}
```

### EnsureSuccessStatusCode (Throws on 4xx/5xx)

```csharp
var response = await _httpClient.GetAsync("/appointments/101");
response.EnsureSuccessStatusCode();  // Throws HttpRequestException if !IsSuccessStatusCode
// Equivalent to:
// if (!response.IsSuccessStatusCode)
//     throw new HttpRequestException(...);
```

### Common Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `HttpRequestException` | Network error, DNS failure | Retry with backoff |
| `OperationCanceledException` | Timeout | Increase timeout or retry |
| `JsonException` | Malformed JSON | Log response, check API |
| `InvalidOperationException` | Already disposed client | Use `using` or singleton |

### Resilient Pattern with Retry

```csharp
public async Task<Appointment> GetAppointmentWithRetryAsync(
    int id,
    CancellationToken cancellationToken)
{
    int retries = 3;
    while (retries > 0)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"appointments/{id}",
                cancellationToken);
            response.EnsureSuccessStatusCode();
            return await JsonSerializer.DeserializeAsync<Appointment>(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex) when (retries > 0)
        {
            retries--;
            _logger.LogWarning(ex, "Request failed, retrying ({remaining} attempts left)", retries);
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, 3 - retries)), cancellationToken);
        }
    }
    
    throw new HttpRequestException("Failed after 3 retries");
}
```

**Pattern**:
1. Try operation
2. If transient error (timeout, 503), retry
3. Exponential backoff: wait 1s, 2s, 4s
4. After max retries, fail permanently

---

## Summary: HttpClient & LINQ Patterns

| Concept | Best Practice |
|---------|---|
| **HttpClient** | Use `IHttpClientFactory`, never `new HttpClient()` |
| **Typed clients** | Encapsulate API logic, inject into services |
| **JSON** | Use `System.Text.Json` (modern), configure `PropertyNameCaseInsensitive` |
| **CancellationToken** | Pass through entire async chain |
| **LINQ deferred** | Remember: queries don't run until enumeration |
| **Materialize** | Use `.ToList()` before multiple iterations |
| **N+1 problem** | Eager load with `.Include()` or use projection |
| **Error handling** | Catch `HttpRequestException`, implement retry logic |

---

## Kotlin Comparison

| Task | Kotlin (Ktor client) | C# (HttpClient) |
|------|-----|-----|
| **Create client** | `HttpClient()` | `IHttpClientFactory.CreateClient()` |
| **GET request** | `httpClient.get()` | `httpClient.GetAsync()` |
| **Parse JSON** | `response.body<Appointment>()` | `JsonSerializer.Deserialize<Appointment>()` |
| **Timeout** | `timeout { requestTimeoutMillis = 5000 }` | `CancellationTokenSource(5000)` |
| **Deferred execution** | `list.filter { }` (eager by default) | `.Where()` (lazy) |
| **Retry** | Ktor interceptors | Manual loop + backoff |

**Key difference**: Kotlin LINQ-like operations (`.filter()`, `.map()`) are usually eager (run immediately). C# LINQ is deferred (runs on enumeration).
