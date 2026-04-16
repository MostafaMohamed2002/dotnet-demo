# REAL-WORLD CONTEXT

## Production Context
In a large enterprise codebase, you will often see "Service Decorators." This is a pattern where you wrap a service to add functionality (like logging or caching) without changing the original service.

### The Clinic Case Study: The "Logging Wrapper"
You have an `IAppointmentService`. You want to log how long every appointment booking takes.

**Junior approach**: Add `_logger.LogInformation(...)` inside every method of `AppointmentService`. (Pollutes business logic with logging).

**Senior approach**: Create a `LoggingAppointmentService` that implements `IAppointmentService` and wraps the original one.
```csharp
public class LoggingAppointmentService : IAppointmentService {
    private readonly IAppointmentService _inner;
    private readonly ILogger<LoggingAppointmentService> _logger;

    public LoggingAppointmentService(IAppointmentService inner, ILogger<LoggingAppointmentService> logger) {
        _inner = inner;
        _logger = logger;
    }

    public async Task Book() {
        _logger.LogInformation("Booking started...");
        await _inner.Book();
        _logger.LogInformation("Booking finished.");
    }
}
```
This is only possible because of DI. You can register this wrapper in `Program.cs` and the controller never knows the difference.

## Common Junior Mistake: Service Locator Anti-Pattern
A junior developer avoids constructor injection and instead does this:
```csharp
public class AppointmentController : ControllerBase {
    public IActionResult Get() {
        // WRONG: This is the "Service Locator" anti-pattern.
        var service = AppDI.ServiceProvider.GetService<IAppointmentService>(); 
        return Ok(service.GetAll());
    }
}
```
**Why this fails code review:**
This hides the dependencies. If you look at the constructor, you have no idea what the controller needs to function. It makes unit testing impossible because you can't easily mock the service. Always use **Constructor Injection**.
