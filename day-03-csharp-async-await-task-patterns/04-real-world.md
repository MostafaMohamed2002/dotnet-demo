# REAL-WORLD CONTEXT — Async/Await in ASP.NET Core + EF Core

## Production Example: Clinic Appointment Service

```csharp
#nullable enable

public class AppointmentService
{
    private readonly ClinicDbContext _db;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(ClinicDbContext db, ILogger<AppointmentService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Standard async method returning Task<T>
    public async Task<AppointmentDetailsDto> GetAppointmentDetailsAsync(
        int appointmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Fetch appointment and related data in parallel
            var aptTask = _db.Appointments
                .Where(a => a.Id == appointmentId)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            var historyTask = _db.Appointments
                .Where(a => a.PatientId == appointmentId && a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.ScheduledTime)
                .Take(5)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Wait for both to complete
            var (appointment, history) = await (aptTask, historyTask);

            if (appointment is null)
                throw new InvalidOperationException("Appointment not found");

            return new AppointmentDetailsDto
            {
                Id = appointment.Id,
                DoctorName = appointment.Doctor!.FullName,
                PatientName = appointment.Patient!.FullName,
                ScheduledTime = appointment.ScheduledTime,
                Status = appointment.Status.DisplayName(),
                RecentHistory = history.Select(a => a.ScheduledTime).ToList()
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("GetAppointmentDetails cancelled");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Appointment not found: {appointmentId}", appointmentId);
            throw;
        }
    }

    // Async method with Task.WhenAny for timeout
    public async Task<AppointmentDetailsDto?> GetAppointmentWithTimeoutAsync(
        int appointmentId,
        TimeSpan timeout)
    {
        var fetchTask = GetAppointmentDetailsAsync(appointmentId);
        var timeoutTask = Task.Delay(timeout);

        var completed = await Task.WhenAny(fetchTask, timeoutTask);

        if (completed == timeoutTask)
        {
            _logger.LogWarning("GetAppointmentDetails timed out after {timeout}ms", timeout.TotalMilliseconds);
            return null;
        }

        return await fetchTask;
    }

    // Cached variant using ValueTask (hot path)
    public ValueTask<Doctor?> GetDoctorWithCacheAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        // Fast path: check in-memory cache
        if (TryGetDoctorFromCache(doctorId, out var cached))
            return new ValueTask<Doctor?>(cached);

        // Slow path: fetch from database
        return new ValueTask<Doctor?>(
            _db.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId, cancellationToken)
                .ConfigureAwait(false)
        );
    }

    // Fire-and-forget with proper error handling
    public void ScheduleReminderNotification(int appointmentId)
    {
        // Discard intent: intentionally not awaiting
        _ = Task.Run(async () =>
        {
            try
            {
                var apt = await GetAppointmentDetailsAsync(appointmentId).ConfigureAwait(false);
                await SendReminderAsync(apt).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule reminder for appointment {appointmentId}", appointmentId);
            }
        });
    }

    private bool TryGetDoctorFromCache(int doctorId, out Doctor? doctor)
    {
        // In-memory cache check (simplified)
        return _cache.TryGetValue($"doctor_{doctorId}", out doctor);
    }

    private async Task SendReminderAsync(AppointmentDetailsDto apt)
    {
        // Notification service call (placeholder)
        await Task.Delay(100);  // Simulate I/O
    }
}
```

### What This Teaches

1. **Async all the way:** Every method that touches the database is `async Task<T>` or `async Task`. No blocking.

2. **ConfigureAwait(false):** Used consistently on library methods (not on ASP.NET Core endpoints, where it's implicit) to signal, "This method doesn't depend on a specific context."

3. **Tuple unpacking with parallel awaits:** `var (apt, history) = await (aptTask, historyTask);` runs both queries in parallel and unpacks results—more efficient than sequential awaits.

4. **CancellationToken propagation:** Passed through the method signature and into EF Core calls. ASP.NET Core automatically provides one; if the client disconnects, the token is signalled.

5. **Task.WhenAny for timeout patterns:** A common pattern in production backends to limit how long an operation can take.

6. **ValueTask for cached values:** If the doctor is already cached, no Task is allocated.

7. **Fire-and-forget with Task.Run and error logging:** Notifications don't block the response; errors are logged explicitly.

---

## ASP.NET Core Controller Integration

```csharp
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentsController(AppointmentService service) => _service = service;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(int id, CancellationToken cancellationToken)
    {
        try
        {
            var details = await _service.GetAppointmentDetailsAsync(id, cancellationToken);
            return Ok(details);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/with-timeout")]
    public async Task<IActionResult> GetAppointmentWithTimeout(int id)
    {
        var details = await _service.GetAppointmentWithTimeoutAsync(id, TimeSpan.FromSeconds(3));
        return details is null ? NotFound() : Ok(details);
    }
}
```

**What's happening:**
- The controller action is `async Task<IActionResult>`.
- It passes the `CancellationToken` from the request to the service.
- If the client disconnects, the token is signalled; EF Core cancels the database operation.
- The response is returned only when the service's `await` completes.

---

## Gotcha: Blocking on Async Code in Tests

```csharp
// ❌ NEVER do this in production tests
[Test]
public void GetAppointment_ReturnsCorrectDoctor()
{
    var result = _service.GetAppointmentDetailsAsync(1).Result;  // BLOCKS
    Assert.That(result.DoctorName, Is.EqualTo("Alice"));
}

// ✅ Use async test methods
[Test]
public async Task GetAppointment_ReturnsCorrectDoctor()
{
    var result = await _service.GetAppointmentDetailsAsync(1);
    Assert.That(result.DoctorName, Is.EqualTo("Alice"));
}
```

Blocking on async code in tests defeats the purpose (no parallelism) and can mask deadlocks.

---

## Another Gotcha: Forgetting ConfigureAwait in Library Code

```csharp
// ❌ Library method without ConfigureAwait(false)
public async Task<Appointment> GetAsync(int id)
{
    return await _db.Appointments.FindAsync(id);  // May capture UI context
}

// ✅ With ConfigureAwait(false)
public async Task<Appointment> GetAsync(int id)
{
    return await _db.Appointments.FindAsync(id).ConfigureAwait(false);
}
```

In ASP.NET Core, this doesn't matter (no context). But if the library is used in a WinForms app, forgetting ConfigureAwait can cause deadlocks.

---

## Production Pattern: Graceful Shutdown with CancellationToken

```csharp
public class BackgroundJobService : IHostedService
{
    private CancellationTokenSource? _cts;
    private Task? _backgroundTask;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = new CancellationTokenSource();
        _backgroundTask = ProcessJobsAsync(_cts.Token);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        if (_backgroundTask is not null)
            await _backgroundTask;
    }

    private async Task ProcessJobsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextJobAsync(cancellationToken).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;  // Graceful shutdown
            }
        }
    }

    private async Task ProcessNextJobAsync(CancellationToken cancellationToken)
    {
        // Process a background job
        await Task.Delay(100, cancellationToken);
    }
}
```

When the app shuts down, `StopAsync` signals the cancellation token; the background loop exits gracefully instead of being forcefully terminated.

---

## Summary

In production .NET backends:
- **Every I/O operation is async:** Database queries, HTTP calls, file I/O.
- **ConfigureAwait(false) is a library convention** to avoid context capture.
- **CancellationToken propagates through the call stack** for graceful cancellation.
- **Async all the way:** Don't block; let the caller await.
- **Task.WhenAll and Task.WhenAny** coordinate multiple operations.
- **ValueTask is rare:** Use Task unless you've profiled and measured allocations.
- **Async void is for events only.** Everything else returns Task.
