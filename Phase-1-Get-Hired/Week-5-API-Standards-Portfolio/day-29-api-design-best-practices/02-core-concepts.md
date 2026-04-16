# CORE CONCEPT BREAKDOWN

## 1. Pagination: The Skip/Take Pattern
Returning thousands of rows is a disaster for both the server and the client. Pagination ensures the server only processes a small chunk of data per request.

### The `PagedResult<T>` Envelope
Never return a raw `List<T>`. Always wrap it in an envelope that tells the client how to paginate.

```csharp
public class PagedResult<T> {
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PagedResult(List<T> data, int page, int pageSize, int totalCount) {
        Data = data;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
```

### Implementing Pagination in LINQ
```csharp
// Request: GET /api/appointments?page=1&pageSize=20
public async Task<ActionResult<PagedResult<AppointmentDto>>> GetAppointments(int page = 1, int pageSize = 20) {
    var query = _context.Appointments.AsNoTracking();

    int totalCount = await query.CountAsync(); // 1. Get total count first
    
    var data = await query
        .Skip((page - 1) * pageSize) // 2. Skip the previous pages
        .Take(pageSize)              // 3. Take only the requested amount
        .Select(a => new AppointmentDto { ... })
        .ToListAsync();

    return Ok(new PagedResult<AppointmentDto>(data, page, pageSize, totalCount));
}
```

## 2. Filtering and Sorting
Filtering allows users to narrow down results. Sorting allows them to organize them. Both must be applied to the `IQueryable` before calling `ToListAsync()`.

### Implementation Pattern
```csharp
// Request: GET /api/appointments?status=Confirmed&sortBy=Date&sortDir=desc
public async Task<IActionResult> GetAppointments(string? status, string? sortBy, string? sortDir) {
    IQueryable<Appointment> query = _context.Appointments.AsNoTracking();

    // Filtering
    if (!string.IsNullOrEmpty(status)) {
        query = query.Where(a => a.Status == status);
    }

    // Sorting
    if (sortBy == "Date") {
        query = sortDir == "desc" 
            ? query.OrderByDescending(a => a.AppointmentDate) 
            : query.OrderBy(a => a.AppointmentDate);
    }

    var data = await query.ToListAsync();
    return Ok(data);
}
```

## 3. Consistent Response Envelopes
A professional API is predictable. If one endpoint returns a raw object and another returns a wrapped object, the frontend developer will hate you.

**The Standard Envelope:**
- **Success**: `200 OK` $\rightarrow$ `{ "data": { ... }, "message": "Success" }`
- **Failure**: `400/500` $\rightarrow$ `ProblemDetails` (RFC 7807).

## 4. API Versioning (The Theory)
As your API grows, you will need to make "Breaking Changes" (e.g., renaming a field from `DoctorName` to `FullName`). If you just change the code, all existing mobile apps will crash.

**The Solution**: Versioning.
- **URL Versioning**: `/api/v1/appointments` and `/api/v2/appointments`.
- **Header Versioning**: `Accept: application/json;v=2.0`.

For your portfolio project, you don't need to implement this, but you must be able to explain in an interview that you know how to prevent breaking changes using versioning.
