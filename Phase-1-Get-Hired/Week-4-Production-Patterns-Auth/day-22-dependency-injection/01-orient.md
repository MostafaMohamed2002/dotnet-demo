# ORIENT

## Why This Topic Exists
In a small project, calling `new AppointmentService()` inside a controller is easy. But as your application grows, `AppointmentService` might depend on `IDoctorRepository`, `IPatientRepository`, and `IEmailService`. If you manually instantiate these, your controllers become bloated with "boilerplate" setup code, and changing a constructor in a low-level service requires you to update every single place that service is instantiated.

**Dependency Injection (DI)** is the architectural solution to this. Instead of a class "reaching out" to create its own dependencies, the dependencies are "injected" into it, typically via the constructor.

## Android/Kotlin Parallels
You are already familiar with this via **Hilt** or **Dagger**.
- **The DI Container $\approx$ Hilt Component**: The system that knows how to instantiate and provide the correct objects.
- **`AddScoped` $\approx$ `@ActivityScoped`**: An object that lives as long as a specific context (in this case, one HTTP request).
- **`AddSingleton` $\approx$ `@Singleton`**: An object that lives for the entire lifetime of the application.
- **Constructor Injection**: Exactly the same as using `@Inject constructor(...)` in Kotlin.

## The Bigger Picture
DI is the foundation of "Decoupling." By depending on interfaces (`IAppointmentService`) rather than concrete classes (`AppointmentService`), you can swap implementations (e.g., swapping a `SqlAppointmentRepository` for a `MockAppointmentRepository` during testing) without touching the controller code. In .NET, DI is not an "add-on"—it is baked into the core of the framework.
