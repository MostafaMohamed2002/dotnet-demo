## Task
Design the foundational database and identity layer for the Clinic Booking API. You must ensure the schema is locked in and the authentication pipeline is functional before moving to business logic.

## Files to Create / Modify
- `Data/ApplicationDbContext.cs` — Define `DbSet` for Users, Doctors, Patients, and Appointments. Configure One-to-One and One-to-Many relationships using Fluent API.
- `Models/User.cs`, `Models/Doctor.cs`, `Models/Patient.cs`, `Models/Appointment.cs` — Domain entities.
- `Services/IAuthService.cs` & `AuthService.cs` — Logic for `Register`, `Login` (returning a JWT), and `ValidateToken`.
- `Controllers/AuthController.cs` — Endpoints for `/auth/register` and `/auth/login`.
- `Program.cs` — Configure `AddAuthentication`, `AddJwtBearer`, and `UseAuthentication`/`UseAuthorization` middleware.
- `appsettings.json` — Add JWT secret keys and SQL Server connection string.

## Expected Output / Behavior
1. **Migrations**: Running `dotnet ef database update` creates the tables in SQL Server.
2. **Registration**: Sending a POST to `/auth/register` creates a user with a specific role.
3. **Authentication**: Sending a POST to `/auth/login` returns a valid JWT.
4. **Authorization**: A protected endpoint (e.g., `/auth/me`) returns 401 Unauthorized without a token and 200 OK with a valid one.

## Do NOT Do This
- **Do not store passwords in plain text.** Use a hashing service.
- **Do not put business logic (like booking an appointment) in the AuthController.** Keep it strictly for identity.
- **Do not use `string` for IDs.** Use `Guid` or `int` for primary keys.

## Self-Review Checklist
- [ ] Does the `Appointment` table have foreign keys to both `DoctorId` and `PatientId`?
- [ ] Is the JWT secret key stored in `appsettings.json` and not hardcoded in C#?
- [ ] Does `Program.cs` have `app.UseAuthentication()` placed **before** `app.UseAuthorization()`?
- [ ] Can I register a user as an "Admin" and another as a "Patient"?
