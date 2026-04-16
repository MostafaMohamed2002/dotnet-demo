# QUIZ

Answer all 5 questions. Paste your answers into the chat when ready.

## Questions

1. **Conceptual**: What is the primary benefit of depending on an interface (e.g., `IAppointmentService`) rather than a concrete class (`AppointmentService`) in a constructor?
2. **Conceptual**: You are creating a service that caches the list of all medical specializations from a database. Since the list rarely changes, you want the service to live for the entire app lifetime. Which DI lifetime should you use?
3. **Code-based**: Identify the bug in the following registration:
   ```csharp
   builder.Services.AddSingleton<ICacheService, CacheService>();
   builder.Services.AddScoped<IDbContextService, DbContextService>();
   // CacheService constructor takes IDbContextService
   ```
4. **Code-based**: What is the a correct way to inject a service into a controller? (Provide a brief snippet).
5. **Scenario**: A developer claims that `AddTransient` is the "safest" lifetime because it prevents any state from being shared between requests. Why is this usually the wrong choice for a web API that uses a database?
