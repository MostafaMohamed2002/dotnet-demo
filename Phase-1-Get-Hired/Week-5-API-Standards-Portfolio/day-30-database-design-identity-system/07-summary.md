# Summary — Day 30: Database Design & Identity System

## What You Learned
- Designed a relational schema for a clinic (Users $\rightarrow$ Doctors/Patients $\rightarrow$ Appointments).
- Implemented EF Core Migrations to synchronize C# models with SQL Server.
- Built a stateless authentication system using JWT (JSON Web Tokens).
- Configured Role-Based Access Control (RBAC) to distinguish between Admin, Doctor, and Patient.
- Established the critical middleware order in `Program.cs` (`Authentication` $\rightarrow$ `Authorization`).
- Applied security best practices regarding password hashing and JWT payload size.

## What This Unlocks
This foundation allows us to build protected API endpoints. We can now write logic that knows *who* is making the request and *what* they are allowed to do.

## Revisit Before Next Session
- Ensure you understand how a `Claim` in a JWT maps to a `Role` in the `[Authorize]` attribute.
- Verify that your database migrations are running cleanly on your local SQL Server instance.
