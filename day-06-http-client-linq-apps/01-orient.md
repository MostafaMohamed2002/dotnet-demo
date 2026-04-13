# Day 6: First Real Code – HTTP Client & LINQ Apps – Orient

## Why This Day Exists

For five days, you've learned the **foundations**:
- C# syntax (properties, nullability, patterns)
- Asynchronous execution (Tasks, async/await)
- Data manipulation (LINQ, interfaces)
- The runtime and ecosystem (CLR, GC, `.csproj`, `dotnet` CLI)

Today, you finally build **real applications** that you'd see in production backend systems.

**Your two projects**:
1. **Appointment API Client**: Fetch appointment data from a REST API over HTTP, parse JSON, handle errors gracefully
2. **Clinic Query Engine**: Query in-memory appointment data using LINQ, understand deferred execution, optimize with eager materialization

Both use patterns you'll repeat daily: HTTP requests, JSON serialization, async/await, LINQ queries, error handling, and dependency injection.

---

## Why These Two Projects?

### App 1: HTTP Client (Connectivity)
**Real-world scenario**: Your backend clinic system needs to fetch data from a third-party appointment booking service. You'll:
- Create an HTTP client with proper dependency injection (`IHttpClientFactory`)
- Handle cancellation tokens for timeouts
- Parse JSON responses into strongly-typed objects
- Implement retry logic with `Polly` (resilience)
- Gracefully handle API errors (rate limits, timeouts, bad data)

**Why it matters**: 99% of backend work involves calling other services. Mastering this pattern is essential.

### App 2: LINQ Query Engine (Data Manipulation)
**Real-world scenario**: You have in-memory appointment data (from a cache, database, or API). You'll:
- Understand deferred execution (queries don't run until enumerated)
- Spot N+1 query problems (and fix them with eager loading)
- Distinguish between `IEnumerable` (in-memory) and `IQueryable` (deferred to database)
- Optimize queries with `.AsEnumerable()`, `.ToList()`, `.FirstOrDefault()`
- Chain complex filters and projections correctly

**Why it matters**: LINQ is your main tool for data manipulation. Getting it wrong causes slow queries and memory leaks.

---

## Bridging from Day 5

Day 5 taught you the **plumbing**. Today you'll **use it**:

- **Project creation**: `dotnet new console --name AppointmentApiClient`
- **Package management**: `dotnet add package HttpClientFactory`, `dotnet add package Polly`
- **Building & running**: `dotnet build`, `dotnet run`
- **Understanding outputs**: Compiled DLLs in `bin/`, debug symbols in `.pdb`

Both apps will be **real, runnable code**—not toy examples. You can copy them into production (with error handling and logging added).

---

## What You'll Learn Today

### 1. **HTTP Client Patterns** (App 1)
   - `System.Net.Http.HttpClient` and why `new HttpClient()` is wrong
   - `IHttpClientFactory` for dependency injection (best practice)
   - Typed HTTP clients (encapsulation)
   - Handling `CancellationToken` (timeouts, graceful shutdown)
   - Parsing JSON with `System.Text.Json` (modern) vs `Newtonsoft.Json` (legacy)
   - Error handling: HTTP status codes, timeouts, network failures

### 2. **LINQ & Deferred Execution** (App 2)
   - `IEnumerable<T>` vs `IQueryable<T>` (in-memory vs deferred)
   - Deferred execution: queries don't run until enumeration
   - Materialization: `.ToList()`, `.FirstOrDefault()`, `.Count()` trigger execution
   - N+1 problem: how lazy loading in loops destroys performance (and how to fix it)
   - Projection (`.Select()`) and filtering (`.Where()`) in chains
   - Complex LINQ expressions vs explicit loops (readability vs performance)

### 3. **Resilience & Production Patterns** (App 1)
   - Retry logic with exponential backoff
   - Timeout configuration
   - Graceful degradation (fallback behavior when API fails)
   - Structured logging (which data to capture for debugging)

### 4. **Integration** (App 3 – Optional)
   - Combining both apps: fetch appointments from API, query with LINQ, cache results
   - Separation of concerns: HTTP layer vs query layer vs business logic

---

## Structure

| File | Purpose |
|------|---------|
| `01-orient.md` | This file – context and overview |
| `02-core-concepts.md` | Technical details on HttpClient, JSON, LINQ, deferred execution |
| `03-hands-on-app-1-http-client.md` | Build an HTTP client app (typed client, error handling) |
| `04-hands-on-app-2-linq.md` | Build a LINQ query engine (deferred execution, optimization) |
| `05-integration-task.md` | Optional: combine both apps |
| `06-quiz.md` | 5 questions on CLR, HttpClient patterns, LINQ concepts |
| `07-summary.md` | Recap and bridge to Week 2 (EF Core, ASP.NET) |

---

## Project Structure (What You'll Build)

```
After today, you'll have two standalone console apps:

AppointmentApiClient/
├─ Program.cs              (HTTP client logic)
├─ AppointmentApiClient.csproj
├─ bin/Debug/net8.0/       (compiled executable)
└─ obj/                    (build internals)

AppointmentQueryEngine/
├─ Program.cs              (LINQ query logic)
├─ AppointmentQueryEngine.csproj
├─ bin/Debug/net8.0/       (compiled executable)
└─ obj/                    (build internals)

(Optional) AppointmentIntegration/
├─ Program.cs              (combined app)
├─ AppointmentIntegration.csproj
└─ ...
```

Each is **standalone and runnable**. You can extract them into your own projects.

---

## Domain Consistency

Both apps use the **same clinic domain** from Days 1–4:

```csharp
public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; }
    public AppointmentStatus Status { get; set; }
}

public enum AppointmentStatus { Scheduled, Completed, Cancelled }
```

Same classes, same relationships. Only the **execution context** differs:
- App 1 fetches them from an HTTP API
- App 2 queries them from in-memory cache

---

## From Kotlin to C#: Practical Patterns

### Kotlin: API Call
```kotlin
val response = httpClient.get("https://clinic-api.local/appointments/101") {
    timeout {
        requestTimeoutMillis = 5000
    }
}.body<Appointment>()
```

### C#: API Call (What You'll Write)
```csharp
using var response = await httpClient.GetAsync(
    "https://clinic-api.local/appointments/101",
    cancellationToken);
var appointment = await response.Content
    .ReadAsAsync<Appointment>(cancellationToken);
```

**Key differences**:
- C# explicit `CancellationToken` (more control, but verbose)
- Kotlin shorthand with builder (cleaner, but less transparent)

### Kotlin: LINQ-like Filtering
```kotlin
appointments
    .filter { it.status == AppointmentStatus.Scheduled }
    .sortedBy { it.appointmentDate }
    .take(5)
    .forEach { println(it) }
```

### C#: LINQ Query
```csharp
var upcoming = appointments
    .Where(a => a.Status == AppointmentStatus.Scheduled)
    .OrderBy(a => a.AppointmentDate)
    .Take(5)
    .ToList();  // Important: materializes here

foreach (var appointment in upcoming)
    Console.WriteLine(appointment);
```

**Nearly identical logic.** C# has `.Where()`, `.OrderBy()`, `.Take()`; Kotlin has `.filter()`, `.sortedBy()`, `.take()`. Same functional paradigm.

---

## Learning Goals for Day 6

By the end of today, you should be able to:

- [ ] Create a typed HTTP client using `IHttpClientFactory`
- [ ] Parse JSON responses with `System.Text.Json`
- [ ] Handle HTTP errors gracefully (status codes, timeouts, network failures)
- [ ] Pass `CancellationToken` through async call chains
- [ ] Explain deferred execution in LINQ (when queries run)
- [ ] Distinguish `IEnumerable` vs `IQueryable` and their implications
- [ ] Spot N+1 problems and fix them with eager loading
- [ ] Chain LINQ queries and understand operator precedence
- [ ] Write both apps from scratch and run them locally
- [ ] Test both apps with sample data

---

## Checklist: What You Should Know by Day 6 EOD

- [ ] `System.Net.Http.HttpClient` (singleton, reusable)
- [ ] `IHttpClientFactory` (DI integration, best practice)
- [ ] Typed HTTP clients (encapsulation, testability)
- [ ] `System.Text.Json` (modern, built-in JSON serialization)
- [ ] `JsonSerializerOptions` and source generators
- [ ] `CancellationToken` (timeouts, cooperative cancellation)
- [ ] Deferred execution (queries don't run until enumerated)
- [ ] Materialization (`.ToList()`, `.FirstOrDefault()`, `.Count()`)
- [ ] IEnumerable vs IQueryable (in-memory vs SQL/deferred)
- [ ] N+1 problem and `.Include()` / `.AsEnumerable()` fixes
- [ ] Complex LINQ chains and readability trade-offs

---

## Tomorrow (Day 7) Starts Week 2

After today's two console apps, Week 2 shifts to **production backend systems**:
- **Day 7**: EF Core (Entity Framework) – database access, migrations, relationships
- **Day 8**: ASP.NET Core APIs – HTTP endpoints, routing, middleware
- **Day 9**: Dependency Injection – wiring up services, configuration
- **Day 10**: Integration – combining all pieces into a real clinic API

You're building the **foundation** today. Week 2 layers on **database and web layers**.

---

## How to Use This Day

1. **Read 02-core-concepts.md** – Technical foundation (30 min)
2. **Do 03-hands-on-app-1-http-client.md** – Build HTTP client (1 hour)
   - Create project, add packages, write code, test with real API
3. **Do 04-hands-on-app-2-linq.md** – Build LINQ engine (1 hour)
   - Create project, write queries, test with sample data
4. **Try 05-integration-task.md** (optional) – Combine both (30 min)
5. **Take 06-quiz.md** – Self-assess (15 min)
6. **Skim 07-summary.md** – Recap (10 min)

**Total time**: 3.5–4 hours

**Come back to this day** if you hit mysterious HTTP errors, slow queries, or questions like "Why isn't my API response parsing?" or "Why is this LINQ query so slow?"

---

## Key Mental Shift for Today

**From theory to practice.** Days 1–5 taught you **how** C# and .NET work. Today you'll apply it:

- C# syntax + patterns → building real applications
- Async/await + Tasks → concurrent HTTP requests
- LINQ + interfaces → querying data elegantly
- CLI + NuGet → reproducible builds and dependencies

**After today, you're not just reading code. You're writing production patterns.**
