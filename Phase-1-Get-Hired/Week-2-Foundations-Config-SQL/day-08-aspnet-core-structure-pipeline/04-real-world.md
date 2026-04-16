# REAL-WORLD CONTEXT

## Production Implementation
In a professional environment, you will rarely see all middleware in `Program.cs`. Instead, developers create **Custom Middleware** for cross-cutting concerns like global exception handling or request logging.

### Clinic API Case Study: The "Patient-Context" Middleware
In a real clinic API, you often need to know the `TenantId` (which clinic branch the request belongs to) across all services. Instead of passing `tenantId` as a parameter to every single method, we use a custom middleware:

1. Middleware extracts `X-Clinic-Id` from the HTTP Header.
2. Middleware injects this ID into a `Scoped` object called `IUserContext`.
3. The `DoctorRepository` simply asks for `IUserContext` via DI and gets the ID.

## The "Gotcha": Middleware Order
The most common junior mistake is placing `app.UseAuthorization()` **before** `app.UseAuthentication()`.

- **Authentication** = "Who are you?"
- **Authorization** = "Are you allowed to be here?"

If you check if someone is allowed to enter the room before you even check who they are, the app will either crash or return a 403 for everyone. **Order is everything in the pipeline.**

## HTTP Status Code Cheat Sheet for .NET
| Code | Method | Meaning | Use Case |
|---|---|---|---|
| 200 | `Ok()` | Success | Generic success |
| 201 | `Created()` | Created | After successful POST of a new Patient |
| 204 | `NoContent()` | No Content | Successful DELETE |
| 400 | `BadRequest()` | Bad Request | Validation failed (e.g., age is -5) |
| 401 | `Unauthorized()` | Unauthenticated | Token is missing or expired |
| 403 | `Forbid()` | Unauthorized | User is logged in, but is a 'Patient' trying to access 'Admin' logs |
| 404 | `NotFound()` | Not Found | Doctor ID 999 doesn't exist |
| 409 | `Conflict()` | Conflict | Attempting to book a slot already taken |
| 422 | `Unprocessable` | Semantics Error | Valid JSON, but logic is wrong (e.g., Appointment date is in the past) |
| 500 | `Problem()` | Server Error | Unhandled exception |
