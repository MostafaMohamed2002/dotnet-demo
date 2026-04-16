## Task

Implement a production-grade error handling and observability pipeline for the Clinic Booking API.

## Files to Create / Modify

1. **Middleware**: `Middleware/ExceptionHandlingMiddleware.cs` and `Middleware/CorrelationIdMiddleware.cs`.
2. **Validation**: `Validators/CreateAppointmentValidator.cs` (implement rules for Date, DoctorId, and PatientId).
3. **Configuration**: Update `Program.cs` to register the middleware and FluentValidation.
4. **Logging**: Update an existing controller to use structured logging for a "Critical" operation (e.g., Booking).

## Expected Output / Behavior

1. **Global Catch**: Intentionally throw a `new Exception("Database Crash!")` in a controller. Verify that the API returns a `ProblemDetails` JSON response with a `500` status, and no stack trace is visible.
2. **Validation Block**: Send a request to create an appointment with a negative `DoctorId`. Verify that the API returns a `400 BadRequest` with a detailed list of validation errors.
3. **Observability**: 
   - Perform a request.
   - Check the logs. Verify that every log line includes a `CorrelationId`.
   - Verify that the logs use named placeholders (e.g., `{UserId}`) and not string interpolation.

## Do NOT Do This

- **Do NOT put `try-catch` blocks in every controller method**. Let the middleware handle it.
- **Do NOT return raw strings as error messages**. Always use `ProblemDetails` or a structured validation response.
- **Do NOT use `Console.WriteLine`** for logging. Use `ILogger<T>`.

## Self-Review Checklist

- [ ] Does the `ExceptionHandlingMiddleware` return a `ProblemDetails` object?
- [ ] Is the `CorrelationIdMiddleware` adding the ID to the response header `X-Correlation-ID`?
- [ ] Does the `CreateAppointmentValidator` contain at least 3 distinct rules?
- [ ] Are all `_logger` calls using the structured message template format?
