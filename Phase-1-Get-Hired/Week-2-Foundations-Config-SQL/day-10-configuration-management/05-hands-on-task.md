# HANDS-ON TASK

## Task
Implement a strongly-typed configuration system for the Clinic API's JWT authentication.

## Files to Create / Modify
- `Models/Configuration/JwtSettings.cs` — Create the POCO class.
- `appsettings.json` — Add the `Jwt` section with `ExpiryMinutes`.
- `Program.cs` — Bind the section to the class.
- `Services/AuthService.cs` — Create a dummy service that injects `IOptions<JwtSettings>` and prints the secret to the console.

## Expected Behavior
1. The app should start without errors.
2. When `AuthService` is called (you can call it in `Program.cs` for a quick test), it should print the value of the secret.
3. **Challenge**: Set the `Secret` using `dotnet user-secrets` and verify that the app picks it up even though it's not in `appsettings.json`.

## Do NOT Do This
- Do not use `Configuration["Jwt:Secret"]` inside `AuthService`. You must use `IOptions<JwtSettings>`.
- Do not commit the actual secret value to `appsettings.json`.

## Self-Review Checklist
- [ ] Does `JwtSettings` have properties that match the JSON keys exactly?
- [ ] Did you use `builder.Services.Configure<T>` in `Program.cs`?
- [ ] Is `AuthService` using constructor injection for `IOptions<JwtSettings>`?
- [ ] Did you verify the "layering" by changing a value in `appsettings.Development.json` and seeing it override `appsettings.json`?
