# REAL-WORLD CONTEXT

## Production Context
In a real enterprise API, you will almost never see `DbContext` used directly in a Controller. Instead, it's wrapped in a **Repository** or **Service** layer.

### The Clinic Case Study: The "Ghost Update"
A junior developer implements a "Get Doctor" endpoint:
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetDoctor(int id) {
    var doc = await _context.Doctors.FindAsync(id);
    doc.LastAccessed = DateTime.UtcNow; // Accidental modification
    return Ok(doc);
}
```
**The Gotcha:** Even though the developer didn't call `SaveChangesAsync()`, if this code were inside a larger transaction or if a middleware calls `SaveChangesAsync` at the end of the request, the `LastAccessed` change is persisted to the DB. This leads to "Ghost Updates" where data changes even when the user didn't intend to save.

**The Professional Fix:** Use `.AsNoTracking()` for all `GET` requests. This tells EF Core: "Do not track this object; any changes made to it in memory should be ignored."

## Common Junior Mistake: Missing `.Include()`
A developer tries to return a Doctor and their Appointments:
```csharp
var doctor = await _context.Doctors.FirstAsync(d => d.Id == 1);
return Ok(new { doctor.Name, Apps = doctor.Appointments }); 
// Apps will be NULL or Empty.
```
**Why this fails:** EF Core does not perform "Join" operations automatically. If you don't explicitly call `.Include(d => d.Appointments)`, the navigation property is not loaded from the database. This is a safeguard to prevent you from accidentally downloading the entire database into RAM.
