# CORE CONCEPT BREAKDOWN

## 1. Global Exception Handling Middleware
Instead of wrapping every controller method in a `try-catch`, we create a custom middleware that sits at the top of the pipeline.

**The Workflow:**
1. Request enters the pipeline.
2. Middleware wraps the `next(context)` call in a `try-catch`.
3. If an exception occurs, the middleware catches it.
4. It logs the error with a stack trace.
5. It returns a standardized `ProblemDetails` response (HTTP 500) to the client.

```csharp
public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            _logger.LogError(ex, "An unhandled exception occurred");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new ProblemDetails {
                Status = 500,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please use the correlation ID to report this."
            });
        }
    }
}
```

## 2. FluentValidation
Validation logic should not live in the Controller or the Service. It should live in a dedicated **Validator** class.

**The "Right Way":**
```csharp
public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto> {
    public CreateAppointmentValidator() {
        RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("A valid Doctor ID is required.");
        RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.Now).WithMessage("Appointment must be in the future.");
        RuleFor(x => x.PatientId).NotEmpty();
    }
}
```
By registering this with `AddFluentValidationAutoValidation()`, the API will automatically return a `400 BadRequest` with a list of all validation errors before the controller method is even executed.

## 3. Structured Logging
Stop using string interpolation (`$"{userId} created..."`) in logs. This creates a unique string for every single request, which makes indexing impossible.

**Wrong (String Interpolation):**
`_logger.LogInformation($"User {userId} updated appointment {apptId}");` 
$\rightarrow$ Log: "User 123 updated appointment 456" (Index: String)

**Right (Structured/Message Template):**
`_logger.LogInformation("User {UserId} updated appointment {ApptId}", userId, apptId);`
$\rightarrow$ Log: "User {UserId} updated appointment {ApptId}" (Index: Template) + Properties: `{UserId: 123, ApptId: 456}`

This allows you to query your logs in a tool like Kibana: `WHERE Properties.UserId = 123`.

## 4. Correlation IDs
In a production system, one request might touch five different services. If you see an error in the logs, how do you find all the other log lines related to that same request?

**The Solution**: A **Correlation ID** (a unique GUID).
1. **Middleware**: Checks for a header `X-Correlation-ID`. If missing, it generates one.
2. **Context**: Stores this ID in `HttpContext.Items` or a `LogContext`.
3. **Logging**: Every log line automatically includes this ID.

**Result**: You can search for `CorrelationId: a1-b2-c3` and see the entire lifecycle of a single request from start to finish.
