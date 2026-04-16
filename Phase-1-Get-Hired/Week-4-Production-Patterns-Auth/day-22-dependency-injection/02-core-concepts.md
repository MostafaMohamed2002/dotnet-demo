# CORE CONCEPT BREAKDOWN

## 1. The DI Container: Building the Object Graph
The DI container is a centralized factory. When a request hits a controller, the container looks at the controller's constructor, sees what it needs, creates those dependencies first, and then "injects" them.

### The "Wrong Way" (Manual Instantiation)
```csharp
public class AppointmentController : ControllerBase {
    public IActionResult Get() {
        // WRONG: The controller is now tightly coupled to a specific implementation.
        var service = new AppointmentService(new AppointmentRepository(new ClinicDbContext())); 
        return Ok(service.GetAll());
    }
}
```

### The "Right Way" (Constructor Injection)
```csharp
public class AppointmentController : ControllerBase {
    private readonly IAppointmentService _appointmentService;

    // The DI container provides this automatically
    public AppointmentController(IAppointmentService appointmentService) {
        _appointmentService = appointmentService;
    }

    public IActionResult Get() => Ok(_appointmentService.GetAll());
}
```

## 2. Service Lifetimes
The lifetime determines when the container creates a new instance and when it reuses an existing one.

### `AddTransient` (The Shortest)
A new instance is created **every time** it is requested.
- **Use case**: Small, stateless utility classes.
- **Warning**: Rarely the right choice for web apps as it creates excessive memory pressure.

### `AddScoped` (The Request Lifetime)
One instance is created per **HTTP Request**. If three different services in the same request need the `DbContext`, they all get the exact same instance.
- **Use case**: Database contexts (`DbContext`), Repositories, Unit of Work.
- **Crucial**: This is the most common lifetime for business logic.

### `AddSingleton` (The Longest)
One instance is created when the app starts and is reused for **every request** until the app shuts down.
- **Use case**: Configuration caches, stateless helper services, memory caches.
- **Warning**: Never put a `DbContext` in a Singleton.

## 3. Registering Interfaces to Implementations
In `Program.cs`, you map the interface (the "What") to the concrete class (the "How").

```csharp
// Program.cs
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

## 4. Captive Dependencies: The Silent Bug
A **Captive Dependency** occurs when a service with a *long* lifetime holds a service with a *short* lifetime.

**The Bug Scenario:**
- `CacheService` is registered as **Singleton**.
- `AppointmentService` is registered as **Scoped**.
- `CacheService` constructor takes `IAppointmentService`.

Because the Singleton is only created once, the Scoped `AppointmentService` is "captured" and kept alive for the entire app lifetime. This means the `DbContext` inside that scoped service never gets disposed, leading to memory leaks and potentially corrupted database connections.

**The Rule**: A service can only depend on services with a lifetime **equal to or longer** than its own.
- Singleton $\rightarrow$ Singleton (OK)
- Scoped $\rightarrow$ Singleton (OK)
- Scoped $\rightarrow$ Scoped (OK)
- Singleton $\rightarrow$ Scoped (**BUG**)
