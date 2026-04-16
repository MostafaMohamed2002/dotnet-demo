# REAL-WORLD CONTEXT

## Production Context
In a high-scale environment, logs are not stored in text files; they are streamed to a "Log Aggregator" (e.g., Seq, ELK Stack, Splunk).

### The Clinic Case Study: The "Silent Failure"
A junior developer implements a `try-catch` in the service layer and just logs the error:
```csharp
try {
    await _repo.SaveAsync(appt);
} catch (Exception ex) {
    _logger.LogError($"Error saving appointment: {ex.Message}");
}
```
**The Bug**: The exception is caught and logged, but it is **swallowed**. The service continues executing, and the Controller returns `200 OK`. The user thinks the appointment was booked, but it actually failed.

**The Professional Fix**: Either let the exception bubble up to the **Global Exception Middleware** (which will return a `500`), or return a `Result.Failure()` that the controller can translate into a proper HTTP error.

## Common Junior Mistake: "Log Interpolation"
A developer writes: `_logger.LogInformation($"Patient {p.Name} visited at {p.Date}");`

**Why this fails code review**: This is a performance and indexing nightmare. If you have 1 million patients, the log aggregator sees 1 million *unique* strings. If you use structured logging (`"Patient {PatientName} visited at {Date}"`), the aggregator sees **one** template and 1 million values. The first approach crashes the indexing engine of most log tools.
