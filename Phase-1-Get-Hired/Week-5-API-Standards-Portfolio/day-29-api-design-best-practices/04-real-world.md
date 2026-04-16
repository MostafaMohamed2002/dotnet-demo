# REAL-WORLD CONTEXT

## Production Context
In high-performance APIs, `Skip/Take` pagination becomes slow as the page number increases. `Skip(1000000).Take(20)` requires the database to scan and discard 1 million rows.

**The Senior Solution: Keyset Pagination (Cursor-based)**
Instead of `page=500`, the client sends the ID of the last item they saw: `?lastId=12345`.
The server then queries: `WHERE Id > 12345 ORDER BY Id LIMIT 20`.
This is always constant-time performance, regardless of the page depth.

### The Clinic Case Study: The "Endless Scroll"
You are implementing a "Recent Appointments" feed for the Doctor's app.

**Junior approach**: Fetch all appointments, sort by date in C#, and return the first 20.
**Result**: As the clinic grows to 100k appointments, the API response time increases linearly. Eventually, the request times out.

**Professional approach**:
1. Use `IQueryable` to filter by `DoctorId`.
2. Use `.OrderByDescending(a => a.Date)`.
3. Use `.Skip().Take()` (or Keyset pagination).
4. Return the `PagedResult` envelope.
**Result**: The API responds in 20ms regardless of whether the doctor has 10 or 10,000 appointments.

## Common Junior Mistake: "Filtering in Memory"
A developer writes:
```csharp
var appointments = await _context.Appointments.ToListAsync(); // Data moved to RAM
var filtered = appointments.Where(a => a.Status == status).ToList(); // Filtered in RAM
```
**Why this fails code review**: This is the Day 18 "Performance Cliff." You are downloading the entire table from the DB to the API server just to throw away 99% of the rows. Always filter on the `IQueryable` before calling `ToListAsync()`.
