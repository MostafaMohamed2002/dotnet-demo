# HANDS-ON TASK — Build LINQ Query Service with Repository Pattern

## Task

Build a **clinic query service** using the repository pattern and LINQ. Implement filtering, pagination, aggregation, and proper interface composition. Demonstrate correct LINQ usage (IQueryable composition, projections, eager loading) and avoid common gotchas (N+1, full-table scans, multiple enumeration).

Your implementation must:
1. Create `IReadRepository<T>` and `IRepository<T>` interfaces with proper method signatures
2. Implement `EfRepository<T>` with `Query()` returning `IQueryable<T>`
3. Build a `AppointmentQueryService` with methods for:
   - Filtering by status and date range (IQueryable composition)
   - Pagination with `Skip()/Take()`
   - Aggregation (count by status)
   - Projection to DTOs (reduce columns)
4. Use `.Include()` to prevent N+1 queries
5. Demonstrate gotchas: multiple enumeration, full-table scans (and how to fix them)
6. Include proper `CancellationToken` propagation

---

## Files to Create / Modify

- `Data/Repositories/IReadRepository.cs` — Read-only interface
- `Data/Repositories/IRepository.cs` — Read/write interface extending IReadRepository
- `Data/Repositories/EfRepository.cs` — EF Core implementation
- `Services/AppointmentQueryService.cs` — Query service with LINQ methods
- `Dtos/AppointmentListDto.cs`, `PaginatedResult.cs`, `AppointmentStatistics.cs` — DTO models
- `Program.cs` — DI registration for repositories and services

---

## Expected Output / Behavior

When you use the API or run tests, you should be able to:

```csharp
var service = /* injected */;

// Filter and project to DTO
var appointments = await service.GetScheduledAppointmentsAsync(dateFrom, dateTo, cancellationToken);

// Paginate results
var page = await service.GetAppointmentsPagedAsync(pageNumber: 1, pageSize: 20, cancellationToken);

// Aggregate in SQL
var stats = await service.GetStatisticsAsync(doctorId: 1, cancellationToken);

// All queries execute as single SQL statements
// No N+1; no full-table scans
```

---

## Do NOT Do This

- **Don't materialize too early:** Don't call `.ToList()` before `.Where()` or `.Select()`. Build the IQueryable chain first, then materialize at the boundary.

- **Don't forget `.Include()` for relations:** If you query appointments and use the `Doctor` navigation property, use `.Include(a => a.Doctor)` to avoid N+1.

- **Don't enumerate multiple times:** Don't call `.Count()`, then `.FirstOrDefault()`, then `.ToList()` on the same IQueryable. Materialize once if you need multiple operations.

- **Don't project to full entities when you need DTOs:** Use `.Select()` to project to a DTO; avoid fetching columns you don't need.

- **Don't use in-memory filtering for database queries:** Don't call `.ToList()` then `.Where()` on a DbSet. Filter in SQL with `.Where()`.

- **Don't forget `CancellationToken`:** Pass it through all async operations.

---

## Self-Review Checklist

- [ ] `IReadRepository<T>` interface created with `GetByIdAsync()`, `Query()`, `CountAsync()` methods
- [ ] `IRepository<T>` extends `IReadRepository<T>` and adds `AddAsync()`, `UpdateAsync()`, `DeleteAsync()`
- [ ] `EfRepository<T>` implements both interfaces; `Query()` returns `IQueryable<T>`
- [ ] `AppointmentQueryService` has at least 3 methods demonstrating different LINQ patterns
- [ ] At least one method uses `.Include()` to load related entities (prevent N+1)
- [ ] At least one method uses `.Select()` to project to a DTO
- [ ] At least one method uses `.Skip().Take()` for pagination
- [ ] At least one method uses `.GroupBy().Select()` for aggregation
- [ ] Service methods accept `CancellationToken cancellationToken = default` and pass it to EF Core calls
- [ ] DTO models created (`AppointmentListDto`, `PaginatedResult<T>`, `AppointmentStatistics`)
- [ ] Code demonstrates and comments on gotchas: multiple enumeration, full-table scan (show both bad and good patterns)
- [ ] All IQueryable chains materialize with `.ToListAsync()` or `.FirstOrDefaultAsync()` (not `.ToList()`)
- [ ] No `.Result` or `.Wait()` blocking on async code
- [ ] `dotnet build` completes with zero warnings and zero errors
- [ ] You can explain when to use IQueryable vs IEnumerable and why it matters for performance
