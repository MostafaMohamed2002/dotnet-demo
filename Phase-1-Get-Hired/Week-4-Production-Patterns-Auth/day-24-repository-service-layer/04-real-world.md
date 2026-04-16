# REAL-WORLD CONTEXT

## Production Context
In a large-scale .NET project, you will see this pattern extended with **Unit of Work**. A Unit of Work ensures that if a Service needs to update three different repositories, all changes are committed to the database in a single transaction.

### The Clinic Case Study: The "Partial Booking" Bug
A junior developer implements booking by calling two different repositories:
```csharp
await _doctorRepo.UpdateAvailability(docId); // Success!
await _apptRepo.AddAppointment(appt);        // FAIL! (DB Timeout)
```
**The Result**: The doctor is marked as "Busy," but the appointment was never created. The data is now inconsistent.

**The Professional Fix**: The Service Layer should manage the transaction. Using EF Core, this is simple because the `DbContext` is already a Unit of Work. The Service calls multiple repository methods, and then calls `SaveChangesAsync()` **once** at the very end.

## Common Junior Mistake: "The Generic Repository Trap"
A developer creates `IRepository<T>` and thinks they've solved everything. Then they need a method `GetHighValuePatientsWithRecentVisits()`.

**The Struggle**: They try to force this into the generic repository by passing a complex `Expression<Func<T, bool>>` as a parameter. The result is a "leaking abstraction"—the Service layer is now writing raw EF Core logic inside a method call, defeating the whole purpose of the repository.

**Rule**: If a query is specific to a domain entity, it belongs in a **Specific Repository**.
