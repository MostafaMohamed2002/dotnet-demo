## Task

Implement the Data Access Layer for the Clinic Booking API using EF Core. You will transition from the manual SQL schema created in Day 13-14 to a Code-First approach.

## Files to Create / Modify

- `Models/User.cs`, `Models/Doctor.cs`, `Models/Patient.cs`, `Models/Appointment.cs` — Define entity classes with proper navigation properties.
- `Data/ClinicDbContext.cs` — Implement the `DbContext` and `DbSet` properties.
- `Program.cs` — Configure the SQL Server connection and `AddDbContext`.
- `appsettings.json` — Add the `ConnectionStrings` section.

## Expected Output / Behavior

1. Run `dotnet ef migrations add InitialCreate` and `dotnet ef database update` to generate the schema.
2. Create a simple "Test Controller" with two endpoints:
   - `GET /test/doctors`: Returns a list of all doctors using `.AsNoTracking()`.
   - `GET /test/doctor/{id}`: Returns a doctor AND their list of appointments using `.Include()`.
3. Verify in SQL Server Management Studio (SSMS) that the tables were created with correct Primary and Foreign Keys.

## Do NOT Do This

- **Do NOT use `Lazy Loading` (proxies)**. It is considered an anti-pattern in web APIs because it can trigger hundreds of hidden database calls during JSON serialization.
- **Do NOT hardcode the connection string** in `Program.cs`. Use `appsettings.json`.
- **Do NOT forget `await`** when calling `SaveChangesAsync()` or `ToListAsync()`.

## Self-Review Checklist

- [ ] Did I use `ICollection<T>` for "Many" sides of relationships?
- [ ] Did I verify that `AsNoTracking()` is used on the list endpoint?
- [ ] Does the `Appointment` entity correctly link both `Doctor` and `Patient`?
- [ ] Did the migration create the tables in the correct order (Users before Doctors/Patients)?
