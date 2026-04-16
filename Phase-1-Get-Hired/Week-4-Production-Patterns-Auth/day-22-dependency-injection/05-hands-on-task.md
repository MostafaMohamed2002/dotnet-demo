## Task

Implement a decoupled architecture for the Clinic Booking API using Dependency Injection.

## Files to Create / Modify

1. `Interfaces/IAppointmentService.cs` — Define the contract for booking logic.
2. `Services/AppointmentService.cs` — Implement the business logic.
3. `Interfaces/IAppointmentRepository.cs` — Define the data access contract.
4. `Repositories/AppointmentRepository.cs` — Implement EF Core data access.
5. `Program.cs` — Register all services with the correct lifetimes.
6. `Controllers/AppointmentController.cs` — Use constructor injection to access the service.

## Expected Output / Behavior

1. The `AppointmentController` should have **zero** calls to the `new` keyword for services or repositories.
2. The `AppointmentService` should depend on `IAppointmentRepository`, not the concrete `AppointmentRepository` class.
3. **Verification**: Start the app and call a GET endpoint. Verify that the `AppointmentController` is successfully instantiated by the DI container.
4. **Lifetime Test**: 
   - Create a `GuidService` (Singleton) that generates a GUID in its constructor.
   - Create a `RequestService` (Scoped) that generates a GUID in its constructor.
   - Inject both into a controller.
   - Refresh the page 5 times.
   - **Observation**: The Singleton GUID should stay the same; the Scoped GUID should change every time you refresh.

## Do NOT Do This

- **Do NOT use `AddTransient`** unless you have a very specific reason. Use `AddScoped` for business logic.
- **Do NOT inject a Scoped service into a Singleton service**.
- **Do NOT use the Service Locator pattern** (injecting `IServiceProvider` and calling `.GetService()`).

## Self-Review Checklist

- [ ] Did I use interfaces for all services and repositories?
- [ ] Are all registrations in `Program.cs` using the correct lifetime (`Scoped` vs `Singleton`)?
- [ ] Is the `AppointmentController` constructor using interface-based injection?
- [ ] Did I verify the Singleton vs Scoped behavior using the GUID test?
