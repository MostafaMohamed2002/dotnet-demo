# REAL-WORLD CONTEXT

## Production Context
In a real-world project, "Integration Day" is essentially the **Hardening Phase**. This is where you move from "it works on my machine" to "it is ready for the staging environment."

### The Clinic Case Study: The "Entity Leak"
A developer returns the `User` entity directly from the API. The `User` entity contains a `PasswordHash` property. Even though the developer didn't explicitly add it to the JSON, a change in the EF Core configuration or a new library update causes the `PasswordHash` to be serialized into the JSON response.

**Result**: A massive security breach.
**The Professional Fix**: **Strict DTO usage**. The `UserResponse` DTO only contains `Username` and `Email`. Even if the `User` entity changes, the DTO remains a safe, explicit contract.

## Common Junior Mistake: "The Migration Mess"
A junior developer makes 10 different changes to the entities, runs `database update`, realizes they made a mistake, deletes the migration files manually, and tries again. This leaves the database and the migrations in an inconsistent state.

**The Professional Fix**: Use `dotnet ef migrations remove` to roll back the last migration if it hasn't been applied to the DB. If it has, create a *new* migration that corrects the error. In production, you **never** delete migration history; you only move forward.
