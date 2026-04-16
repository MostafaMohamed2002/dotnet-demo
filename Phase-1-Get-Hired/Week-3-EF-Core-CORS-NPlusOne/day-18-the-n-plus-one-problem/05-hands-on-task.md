## Task

Diagnose and fix an N+1 performance bug in a simulated Clinic API.

## Files to Create / Modify

1. `appsettings.Development.json` — Update logging levels to see SQL commands.
2. `Controllers/DoctorController.cs` — Create a "Broken" endpoint and a "Fixed" endpoint.

## Expected Output / Behavior

1. **The "Broken" Endpoint (`GET /doctors/broken`)**:
   - Fetch all doctors using `.ToListAsync()`.
   - Loop through them and access `doctor.Appointments.Count`.
   - **Verification**: Observe the console logs. You should see one query for doctors, followed by many queries for appointments.
2. **The "Fixed" Endpoint (`GET /doctors/fixed`)**:
   - Implement the same logic but use `.Include(d => d.Appointments)` or a `.Select()` projection.
   - **Verification**: Observe the console logs. You should see exactly **one** SQL query regardless of the number of doctors.
3. **The "Memory Filter" Test**:
   - Create an endpoint that filters patients by city.
   - Implement it once using `.ToList().Where(...)` and once using `.Where(...).ToList()`.
   - Compare the SQL generated in the logs. One should have a `WHERE` clause; the other should be a `SELECT *`.

## Do NOT Do This

- **Do NOT use Lazy Loading**. Do not add `virtual` to your navigation properties or install the `Microsoft.EntityFrameworkCore.Proxies` package.
- **Do NOT simply "increase the timeout"** or "add more RAM." The fix is architectural, not hardware-based.

## Self-Review Checklist

- [ ] Did I successfully enable `Microsoft.EntityFrameworkCore.Database.Command` logging?
- [ ] Can I point to the exact line in the log where the "N" queries start?
- [ ] Does the fixed endpoint result in a single SQL statement?
- [ ] Does the `IQueryable` filter result in a SQL `WHERE` clause?
