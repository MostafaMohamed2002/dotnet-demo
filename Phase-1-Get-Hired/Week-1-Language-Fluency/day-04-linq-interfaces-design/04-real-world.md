# REAL-WORLD CONTEXT — LINQ and Repositories in a Clinic API

## Production Example: Clinic Repository and Query Service

```csharp
#nullable enable

// ===== Interfaces =====

public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<T> Query();  // Return IQueryable for composition
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}

// ===== Implementation =====

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly ClinicDbContext _context;

    public EfRepository(ClinicDbContext context) => _context = context;

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public virtual IQueryable<T> Query()
    {
        return _context.Set<T>();  // Return IQueryable for composition
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().CountAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ===== Service Layer (Composition) =====

public class AppointmentQueryService
{
    private readonly IReadRepository<Appointment> _appointmentRepo;
    private readonly IReadRepository<Doctor> _doctorRepo;

    public AppointmentQueryService(
        IReadRepository<Appointment> appointmentRepo,
        IReadRepository<Doctor> doctorRepo)
    {
        _appointmentRepo = appointmentRepo;
        _doctorRepo = doctorRepo;
    }

    // ✅ IQueryable composition: filtering happens in SQL
    public async Task<List<AppointmentListDto>> GetScheduledAppointmentsAsync(
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken = default)
    {
        // Build IQueryable chain without materializing
        var query = _appointmentRepo.Query()
            .Where(a => a.Status == AppointmentStatus.Scheduled)
            .Where(a => a.ScheduledTime >= dateFrom && a.ScheduledTime <= dateTo)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .OrderBy(a => a.ScheduledTime)
            .Select(a => new AppointmentListDto
            {
                Id = a.Id,
                DoctorName = a.Doctor!.FullName,
                PatientName = a.Patient!.FullName,
                ScheduledTime = a.ScheduledTime,
                Status = a.Status.DisplayName()
            });

        // Execution happens HERE with ToListAsync
        return await query.ToListAsync(cancellationToken);
    }

    // ✅ Pagination pattern
    public async Task<PaginatedResult<AppointmentListDto>> GetAppointmentsPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _appointmentRepo.Query()
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .OrderByDescending(a => a.ScheduledTime);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AppointmentListDto
            {
                Id = a.Id,
                DoctorName = a.Doctor!.FullName,
                PatientName = a.Patient!.FullName,
                ScheduledTime = a.ScheduledTime,
                Status = a.Status.DisplayName()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AppointmentListDto>
        {
            Items = items,
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    // ✅ Aggregation in SQL
    public async Task<AppointmentStatistics> GetStatisticsAsync(
        int doctorId,
        CancellationToken cancellationToken = default)
    {
        var stats = await _appointmentRepo.Query()
            .Where(a => a.DoctorId == doctorId)
            .GroupBy(a => a.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new AppointmentStatistics
        {
            Total = stats.Sum(s => s.Count),
            Scheduled = stats.FirstOrDefault(s => s.Status == AppointmentStatus.Scheduled)?.Count ?? 0,
            Completed = stats.FirstOrDefault(s => s.Status == AppointmentStatus.Completed)?.Count ?? 0,
            Cancelled = stats.FirstOrDefault(s => s.Status == AppointmentStatus.Cancelled)?.Count ?? 0
        };
    }

    // ❌ GOTCHA: Multiple enumeration
    public async Task<ClinicReport> GetReportBadAsync(CancellationToken cancellationToken)
    {
        var query = _appointmentRepo.Query()
            .Where(a => a.Status == AppointmentStatus.Completed);

        // ❌ Query executes #1
        var count = await query.CountAsync(cancellationToken);

        // ❌ Query executes #2 (same query sent to database again!)
        var latest = await query
            .OrderByDescending(a => a.ScheduledTime)
            .FirstOrDefaultAsync(cancellationToken);

        return new ClinicReport { Count = count, Latest = latest };
    }

    // ✅ FIXED: Materialize once
    public async Task<ClinicReport> GetReportGoodAsync(CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepo.Query()
            .Where(a => a.Status == AppointmentStatus.Completed)
            .ToListAsync(cancellationToken);

        // Everything now in memory; no more database queries
        var count = appointments.Count;
        var latest = appointments.OrderByDescending(a => a.ScheduledTime).FirstOrDefault();

        return new ClinicReport { Count = count, Latest = latest };
    }

    // ❌ GOTCHA: Full-table scan
    public async Task<List<Doctor>> GetDoctorsBySpecialtyBadAsync(
        string specialty,
        CancellationToken cancellationToken)
    {
        // ❌ Loads ALL doctors into memory, then filters
        var doctors = await _doctorRepo.Query()
            .ToListAsync(cancellationToken);  // ← Fetch all rows

        return doctors
            .Where(d => d.Specialty == specialty)  // ← Filter in memory (full-table scan)
            .ToList();
    }

    // ✅ FIXED: Filter in SQL
    public async Task<List<Doctor>> GetDoctorsBySpecialtyGoodAsync(
        string specialty,
        CancellationToken cancellationToken)
    {
        return await _doctorRepo.Query()
            .Where(d => d.Specialty == specialty)  // ← WHERE in SQL (uses index)
            .ToListAsync(cancellationToken);
    }
}

// ===== DTO and Result Models =====

public class AppointmentListDto
{
    public int Id { get; set; }
    public string DoctorName { get; set; } = null!;
    public string PatientName { get; set; } = null!;
    public DateTime ScheduledTime { get; set; }
    public string Status { get; set; } = null!;
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class AppointmentStatistics
{
    public int Total { get; set; }
    public int Scheduled { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}
```

### What This Teaches

1. **IQueryable is composable:** You can return `IQueryable<T>` from repositories and build on it in services. The entire chain translates to a single SQL query.

2. **Projection reduces data:** The `.Select()` to DTO translates to `SELECT` in SQL, fetching only needed columns.

3. **Include for eager loading:** `.Include()` prevents N+1 queries by loading related data in the same query.

4. **Aggregation in SQL:** `.GroupBy().Select()` for statistics runs on the database, not in memory.

5. **Pagination with Skip/Take:** The pattern `query.Skip((pageNumber-1)*pageSize).Take(pageSize)` translates to SQL `OFFSET/FETCH`.

6. **Multiple enumeration gotcha:** If you call `.Count()` then `.FirstOrDefault()` on the same IQueryable, you hit the database twice. Materialize once to avoid this.

7. **Full-table scan pitfall:** Calling `.ToList()` before `.Where()` loads all rows into memory, defeating indexes.

---

## ASP.NET Core Controller Integration

```csharp
[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentQueryService _queryService;

    public AppointmentsController(AppointmentQueryService queryService) => _queryService = queryService;

    [HttpGet]
    public async Task<IActionResult> GetScheduled(
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo,
        CancellationToken cancellationToken)
    {
        var appointments = await _queryService.GetScheduledAppointmentsAsync(dateFrom, dateTo, cancellationToken);
        return Ok(appointments);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _queryService.GetAppointmentsPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }
}
```

---

## Summary

In production backends:
- **Repositories return `IQueryable<T>`** to enable service composition and deferred execution.
- **Services build LINQ chains and materialize at the boundary** (controller, message handler).
- **Projections (`.Select()`) to DTOs** reduce data transfer and translate to SQL `SELECT` clauses.
- **Aggregations (`.GroupBy().Select()`)** run on the database, not in memory.
- **Pagination (`.Skip().Take()`)** translates to `OFFSET/FETCH`.
- **N+1 prevention:** Use `.Include()` or materialize once before multiple enumerations.
- **Full-table scan prevention:** Filter with `.Where()` before `.ToList()`.
