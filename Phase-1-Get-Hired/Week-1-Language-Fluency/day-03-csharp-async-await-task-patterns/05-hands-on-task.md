# HANDS-ON TASK — Build Async Clinic Service with Proper Task Patterns

## Task

Extend your clinic domain with an **async appointment service** that properly uses `Task`, `ConfigureAwait(false)`, `CancellationToken`, and parallel task coordination. Build a service that demonstrates real-world async patterns found in production ASP.NET Core backends.

Your service must:
1. Use `async Task<T>` for all database operations (no blocking)
2. Apply `ConfigureAwait(false)` consistently on library methods
3. Support `CancellationToken` propagation through method signatures
4. Use `Task.WhenAll` or tuple unpacking to run operations in parallel
5. Include at least one `ValueTask<T>` pattern for cached data
6. Implement proper error handling with logging
7. Demonstrate safe fire-and-forget pattern (if applicable)

---

## Files to Create / Modify

- `Services/AppointmentService.cs` — Core async service with database queries, parallel operations, caching
- `Controllers/AppointmentsController.cs` — ASP.NET Core controller using the service with CancellationToken
- `Data/SampleData.cs` — Seed data for testing (Doctor, Patient, Appointment entities)
- `Program.cs` — Add DI wiring for the service and logging

---

## Expected Output / Behavior

When you run the API, you should be able to:

```csharp
// Service method with parallel queries
var details = await _service.GetAppointmentDetailsAsync(1, cancellationToken);
// Fetches appointment and recent history in parallel

// Cached retrieval using ValueTask
var doctor = await _service.GetDoctorWithCacheAsync(1);
// Fast path if cached; slow path if not

// Timeout pattern with WhenAny
var details = await _service.GetAppointmentWithTimeoutAsync(1, TimeSpan.FromSeconds(3));

// Controller endpoint receives CancellationToken from request
GET /api/appointments/1 => 200 OK with appointment details
```

**No blocking on `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`** in service or controller code.

**All async operations properly propagate `CancellationToken`** through the call chain.

**ConfigureAwait(false)** applied to library method calls (not ASP.NET Core endpoints).

---

## Do NOT Do This

- **No blocking on async code:** Don't use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` anywhere in service or controller code.

- **No async void (except event handlers):** All service methods return `Task` or `Task<T>`, never `async void`.

- **Don't forget CancellationToken propagation:** If a method signature accepts `CancellationToken`, pass it to all async operations inside.

- **Don't apply ConfigureAwait(false) to ASP.NET Core endpoints:** Apply it to library methods and service code, but not directly on the request handler (it's implicit).

- **Don't store ValueTask instances:** If you use ValueTask, either await it immediately or convert to Task via `.AsTask()`.

- **Don't block on I/O in loops:** If you're fetching multiple items, use `Task.WhenAll` or `await foreach` (async streams) instead of `for` loops with `await` inside.

---

## Self-Review Checklist

- [ ] `AppointmentService` class created with async methods returning `Task<T>` or `Task`
- [ ] All database operations use `ConfigureAwait(false)` (except ASP.NET Core endpoint code)
- [ ] At least one method accepts `CancellationToken cancellationToken = default` and passes it to EF Core calls
- [ ] At least one method uses `Task.WhenAll` or tuple unpacking to run multiple operations in parallel
- [ ] At least one method uses `ValueTask<T>` for a cached/fast-path scenario
- [ ] `AppointmentsController` is created and returns `async Task<IActionResult>`
- [ ] Controller action passes `CancellationToken cancellationToken` from the request to the service
- [ ] No `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` in any service or controller code
- [ ] No `async void` methods (except if using event handlers, which aren't in this task)
- [ ] Exception handling includes logging for key scenarios (not found, timeout, cancellation)
- [ ] You can explain why ConfigureAwait(false) is applied where it is and not applied elsewhere
- [ ] `dotnet build` completes with zero warnings and zero errors
- [ ] You can explain the difference between Task and ValueTask, and when to use each
